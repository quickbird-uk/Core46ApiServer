using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;
using Qb.Poco;

namespace Qb.Core46Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public partial class SyncController : Controller
    {
        private readonly QbDbContext _db;

        public SyncController(QbDbContext db)
        {
            _db = db;
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
            var globals = new Poco.Global.SyncData(cropTypes, parameters, placements, subSystems, sensorTypes);

            return new JsonResult(globals);
        }

        /// <summary>
        /// All user data including Person that has been updated after a specified date.
        /// </summary>
        /// <param name="startUnixUtcTimeSeconds">The earliest updated time to include, in UTC seconds since Unix epoch.</param>
        /// <returns>All user data apart from SensorHistory; people, locations, devies, cropCycles and sensors. Also sends serverDateTime just before the queries to get the data are made.</returns>
        public async Task<IActionResult> GetUserData(long? startUnixUtcTimeSeconds)
        {
            var serverDateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var startDateTime = DateTimeOffset.FromUnixTimeSeconds(startUnixUtcTimeSeconds ?? 0);
            var people = await _db.People.Where(p => p.UpdatedAt > startDateTime).ToArrayAsync();

            var locations = await _db.Locations.Where(l => l.UpdatedAt > startDateTime).ToArrayAsync();
            var devices = await _db.Devices.Where(d => d.UpdatedAt > startDateTime).ToArrayAsync();
            var cropcycles = await _db.CropCycles.Where(c => c.UpdatedAt > startDateTime).ToArrayAsync();
            var sensors = await _db.Sensors.Where(s => s.UpdatedAt > startDateTime).ToArrayAsync();

            var userData = new Poco.User.SyncData(serverDateTime, people, locations, devices, cropcycles, sensors);
            return new JsonResult(userData);
        }

        public async Task<IActionResult> SetGlobals([FromBody] Poco.Global.SyncData syncData)
        {
            await _db.CropTypes.AddOrUpdateRange(syncData.CropTypes, (a, b) => a.Name == b.Name);
            await _db.Placements.AddOrUpdateRange(syncData.Placements, (a, b) => a.Id == b.Id);
            await _db.Parameters.AddOrUpdateRange(syncData.Parameters, (a, b) => a.Id == b.Id);
            await _db.Subsystems.AddOrUpdateRange(syncData.SubSystems, (a, b) => a.Id == b.Id);
            await _db.SensorTypes.AddOrUpdateRange(syncData.SensorTypes, (a, b) => a.Id == b.Id);
            return new OkResult();
        }
    }
}