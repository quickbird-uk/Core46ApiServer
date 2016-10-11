using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;
using Qb.Core46Api.Vars;
using Qb.Poco.User;
using SyncData = Qb.Poco.Global.SyncData;

namespace Qb.Core46Api.Controllers
{
    /// <summary>Controller for upload and download of data to programs.</summary>
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class SyncController : Controller
    {
        private readonly QbDbContext _db;
        private readonly ILogger<SyncController> _logger;
        private readonly OpenIddictUserManager<QbUser> _userManager;

        /// <summary>Initialise with a dbContext, per request and a logger for errors.</summary>
        /// <param name="db">Database context, should last per request.</param>
        /// <param name="loggerFactory">ASP Core logger.</param>
        /// <param name="userManager">ASP Identity user manager.</param>
        public SyncController(QbDbContext db, ILoggerFactory loggerFactory, OpenIddictUserManager<QbUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<SyncController>();
        }

        /// <summary>A complete dump of all global data, this shouldn't change much.</summary>
        /// <returns>A single json object containing croptTypes, parameters, placements, subSystems and sensorTypes.</returns>
        [AllowAnonymous]
        public async Task<IActionResult> GetGlobals()
        {
            var cropTypes = await _db.CropTypes.ToArrayAsync();
            var parameters = await _db.Parameters.ToArrayAsync();
            var placements = await _db.Placements.ToArrayAsync();
            var subSystems = await _db.Subsystems.ToArrayAsync();
            var sensorTypes = await _db.SensorTypes.ToArrayAsync();
            var globals = new SyncData(cropTypes, parameters, placements, subSystems, sensorTypes);

            return new JsonResult(globals);
        }

        /// <summary>All user data including Person that has been updated after a specified date.</summary>
        /// <param name="startUnixUtcTimeSeconds">The earliest updated time to include, in UTC seconds since Unix epoch.</param>
        /// <returns>All user data apart from SensorHistory; people, locations, devies, cropCycles and sensors. Also sends
        ///     serverDateTime just before the queries to get the data are made.</returns>
        public async Task<IActionResult> GetUserData(long? startUnixUtcTimeSeconds)
        {
            // Get the current server time before any database requests are started.
            // The client will regard itself be up-to-date to this time.
            var serverDateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var startDateTime = DateTimeOffset.FromUnixTimeSeconds(startUnixUtcTimeSeconds ?? 0);

            var user = await _userManager.GetUserAsync(User);
            var userGuid = new Guid(user.Id);

            // This will be an array with either 1 or 0 entries. This is more conventient than not using an array.
            var people = await _db.People
                .Where(p => p.Id == userGuid)
                .Where(p => p.UpdatedAt >= startDateTime).ToArrayAsync();

            var locations = await _db.Locations
                .Where(l => l.PersonId == userGuid)
                .Where(l => l.UpdatedAt >= startDateTime).ToArrayAsync();

            var locationIds = locations.Select(l => l.Id);

            var devices = await _db.Devices
                .Where(d => locationIds.Contains(d.LocationId))
                .Where(d => d.UpdatedAt >= startDateTime).ToArrayAsync();

            var cropcycles = await _db.CropCycles
                .Where(c => locationIds.Contains(c.LocationId))
                .Where(c => c.UpdatedAt >= startDateTime).ToArrayAsync();

            var cropCycleIds = cropcycles.Select(c => c.Id);
            var sensors = await _db.Sensors
                .Where(s => cropCycleIds.Contains(s.Id))
                .Where(s => s.UpdatedAt >= startDateTime).ToArrayAsync();

            var userData = new Poco.User.SyncData(serverDateTime, people, locations, devices, cropcycles, sensors);
            return new JsonResult(userData);
        }

        [Authorize(Roles = Roles.EditGlobalData)]
        public async Task<IActionResult> SetGlobals([FromBody] SyncData syncData)
        {
            try
            {
                await _db.CropTypes.AddOrUpdateRange(syncData.CropTypes, (a, b) => a.Name == b.Name);
                await _db.Placements.AddOrUpdateRange(syncData.Placements, (a, b) => a.Id == b.Id);
                await _db.Parameters.AddOrUpdateRange(syncData.Parameters, (a, b) => a.Id == b.Id);
                await _db.Subsystems.AddOrUpdateRange(syncData.SubSystems, (a, b) => a.Id == b.Id);
                await _db.SensorTypes.AddOrUpdateRange(syncData.SensorTypes, (a, b) => a.Id == b.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(LogIds.EditGlobalData, e, "Failed to SetGlobals.");
                return BadRequest("Data invalid or corrupt.");
            }
            return new OkResult();
        }

        [Authorize(Roles = Roles.EditUserData)]
        public async Task<IActionResult> SetUserData(Poco.User.SyncData syncData)
        {
            var now = DateTimeOffset.UtcNow;
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            if (syncData.People.Any() && (syncData.People.First().Id != userId))
                return BadRequest("Non-matching user id.");
            if (syncData.Locations.Any(l => l.PersonId != userId))
                return BadRequest("Can't edit data with non-matching user.");

            // These two requre a db query to make sure they have valid locations (owned by them).
            if (syncData.Devices.Any() || syncData.CropCycles.Any() || syncData.Sensors.Any())
            {
                var existingLocations = await _db.Locations.Where(l => l.PersonId == userId).ToListAsync();
                // The personId on the syncData locations has been checked already, new locations sent are valid too.
                var existingLocationIds = existingLocations.Concat(existingLocations).Select(l => l.Id).ToList();
                if (!syncData.Devices.All(d => existingLocationIds.Contains(d.LocationId)) ||
                    !syncData.CropCycles.All(c => existingLocationIds.Contains(c.LocationId)))
                    return BadRequest("Can't edit data with non-matching user.");

                if (syncData.Sensors.Any())
                {
                    var validDeviceIds = (await _db.Devices.Where(d => existingLocationIds.Contains(d.LocationId))
                            .Select(d => d.Id).ToListAsync())
                        .Concat(syncData.Devices.Select(d => d.Id));
                    if (!syncData.Sensors.All(s => validDeviceIds.Contains(s.DeviceId)))
                        return BadRequest("Can't edit data with non-matching user.");
                }
            }

            // The data is valid so save it.
            foreach (var person in syncData.People)
                person.UpdatedAt = now;
            SetUpdatedTime(syncData.Locations, now);
            SetUpdatedTime(syncData.Devices, now);
            SetUpdatedTime(syncData.CropCycles, now);
            SetUpdatedTime(syncData.Sensors, now);
            await _db.People.AddOrUpdateRange(syncData.People, (a, b) => a.Id == b.Id);
            await _db.Locations.AddOrUpdateRange(syncData.Locations, (a, b) => a.Id == b.Id);
            await _db.Devices.AddOrUpdateRange(syncData.Devices, (a, b) => a.Id == b.Id);
            await _db.CropCycles.AddOrUpdateRange(syncData.CropCycles, (a, b) => a.Id == b.Id);
            await _db.Sensors.AddOrUpdateRange(syncData.Sensors, (a, b) => a.Id == b.Id);
            await _db.SaveChangesAsync();

            return new OkResult();
        }

        private void SetUpdatedTime(IEnumerable<BaseEntity> entities, DateTimeOffset updateTime)
        {
            foreach (var entity in entities)
                entity.UpdatedAt = updateTime;
        }


    }
}