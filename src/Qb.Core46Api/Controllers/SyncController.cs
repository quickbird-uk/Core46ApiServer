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
        public async Task<IActionResult> SetUserData([FromBody] Poco.User.SyncData syncData)
        {
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

            // The updatedAt times are automatically set by savechanges.
            await _db.People.AddOrUpdateRange(syncData.People, (a, b) => a.Id == b.Id);
            await _db.Locations.AddOrUpdateRange(syncData.Locations, (a, b) => a.Id == b.Id);
            await _db.Devices.AddOrUpdateRange(syncData.Devices, (a, b) => a.Id == b.Id);
            await _db.CropCycles.AddOrUpdateRange(syncData.CropCycles, (a, b) => a.Id == b.Id);
            await _db.Sensors.AddOrUpdateRange(syncData.Sensors, (a, b) => a.Id == b.Id);
            await _db.SaveChangesAsync();

            return new OkResult();
        }

        /// <summary>Gets all histories between and including the datetime given.</summary>
        /// <param name="fromDateTimeUnixSeconds">The earliest time to include datapoints.</param>
        /// <param name="toUtcDateTimeUnixSeconds">The latest time to include dataopints.</param>
        /// <returns>All the histories between the specified times.</returns>
        public async Task<IActionResult> GetUserHistories(long fromDateTimeUnixSeconds, long toUtcDateTimeUnixSeconds)
        {
            // Filter by the UTC Date (time 00:00:00 (==12:00am)).
            // All times are converted to the starting time of the day.
            var fromDateTime = DateTimeOffset.FromUnixTimeSeconds(fromDateTimeUnixSeconds);
            var fromDate = fromDateTime.Date;
            var toDateTime = DateTimeOffset.FromUnixTimeSeconds(toUtcDateTimeUnixSeconds);
            var toDate = toDateTime.Date;

            // Prepare to filter by locations owned by the user.
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            var locationIds = await _db.Locations.Where(l => l.PersonId == userId).Select(l => l.Id).ToListAsync();

            var data =
                await
                    _db.SensorHistories.Where(
                            h => locationIds.Contains(h.LocationId) && (h.UtcDate >= fromDate) && (h.UtcDate <= toDate))
                        .ToListAsync();

            var needsSlicing = data.Where(h => (h.UtcDate == fromDate) || (h.UtcDate == toDate));
            foreach (var history in needsSlicing)
            {
                var sliced = SensorDatapoint.Slice(history.RawData, fromDateTime, toDateTime);
                history.RawData = sliced;
            }

            return new JsonResult(data);
        }


        [Authorize(Roles = Roles.EditUserData)]
        public async Task<IActionResult> SetUserHistories([FromBody] List<SensorHistory> histories)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            var locationIds = await _db.Locations.Where(l => l.PersonId == userId).Select(l => l.Id).ToListAsync();

            if (histories.Any(h => !locationIds.Contains(h.LocationId)))
            {
                var message = "Cant save histories at locations not owned by user.";
                _logger.LogError(message);
                return BadRequest(message);
            }

            // Composite keys aren't easy to turn into a query.
            var historyIds = histories.Select(h => $"{h.SensorId},{h.UtcDate.Ticks}");

            var existsInDb = await _db.SensorHistories
                .Where(h => historyIds.Contains($"{h.SensorId},{h.UtcDate.Ticks}"))
                .ToListAsync();

            var itemsToUpdate = new List<SensorHistory>();
            foreach (var dbHistory in existsInDb)
            {
                // Removed the already existing item from the histories recieved from the API.
                var recievedHist =
                    histories.First(h => (h.UtcDate == dbHistory.UtcDate) && (h.SensorId == dbHistory.SensorId));
                histories.Remove(recievedHist);

                var mergedData = SensorDatapoint.Merge(dbHistory.RawData, recievedHist.RawData);

                // Merge the data in the newly recieved data which should have an updated UploadedAt.
                recievedHist.RawData = mergedData;
                if (recievedHist.UploadedAt < DateTimeOffset.Now - TimeSpan.FromMinutes(5))
                    return BadRequest("UploadedAt time was not set or significant lag has occured.");
                itemsToUpdate.Add(recievedHist);
            }

            // The remaining items left in histories must all be new.
            _db.SensorHistories.AddRange(histories);
            _db.SensorHistories.UpdateRange(itemsToUpdate);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}