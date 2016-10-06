using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;
using Qb.Poco.Global;

namespace Qb.Core46Api.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private const string ApiUrl = "https://ghapi46azure.azurewebsites.net/api/";
        private readonly QbDbContext _db;

        public ImportController(QbDbContext db)
        {
            _db = db;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Authed(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", token);

                var message = await client.GetAsync("CropCycles");
                var cropCycles = await message.Content.ReadAsStringAsync();

                return Content(cropCycles);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Open()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);

                var parameters = await GetAndParseJson("parameters", client);
                var subsystems = await GetAndParseJson("subsystems", client);
                var croptypes = await GetAndParseJson("croptypes", client);
                var sensorTypes = await GetAndParseJson("sensorTypes", client);
                var placements = await GetAndParseJson("placements", client);

                await _db.Parameters.AddOrUpdateRange(ConvertParameters(parameters), (p1, p2) => p1.Id == p2.Id);
                await _db.Subsystems.AddOrUpdateRange(ConvertSubsystems(subsystems), (p1, p2) => p1.Id == p2.Id);
                await _db.CropTypes.AddOrUpdateRange(ConvertCropTypes(croptypes), (p1, p2) => p1.Name == p2.Name);
                await _db.SensorTypes.AddOrUpdateRange(ConvertSensorTypes(sensorTypes), (p1, p2) => p1.Id == p2.Id);
                await _db.Placements.AddOrUpdateRange(ConvertPlacements(placements), (p1, p2) => p1.Id == p2.Id);

                await _db.SaveChangesAsync();

                return Content($"[{parameters},{subsystems},{croptypes},{sensorTypes},{placements}]");
            }
        }

        private IEnumerable<Placement> ConvertPlacements(JArray placements)
        {
            return placements.Select(p => new Placement
            {
                Id = Convert.ToInt64((string) p["ID"]),
                Name = (string) p["Name"]
            });
        }

        private IEnumerable<SensorType> ConvertSensorTypes(JArray sensorTypes)
        {
            return sensorTypes.Select(st => new SensorType
            {
                Id = Convert.ToInt64((string) st["ID"]),
                ParameterId = Convert.ToInt64((string) st["ParamID"]),
                PlacementId = Convert.ToInt64((string) st["PlaceID"]),
                SubsystemId = Convert.ToInt64((string) st["SubsystemID"])
            });
        }

        private IEnumerable<CropType> ConvertCropTypes(JArray croptypes)
        {
            return croptypes.Select(ct =>
            {
                var createdAt = (string) ct["CreatedAt"];
                Debug.WriteLine(CultureInfo.CurrentCulture);
                Debug.WriteLine($"Parseing >> 6/02/2016 00:00:00 <<");
                Debug.WriteLine($"result: {DateTimeOffset.Parse(" 6/02/2016 00:00:00 ")}");
                string x = " " + createdAt + " ";
                Debug.WriteLine($"Parseing >>{x}<<");
                Debug.WriteLine($"result: {DateTimeOffset.Parse(x)}");

                Debug.WriteLine($"GOING TO PARSE \n>>{createdAt}<<");
                var parsed = DateTimeOffset.Parse(createdAt.ToString());
                Debug.WriteLine("parsed:" + parsed);
                return new CropType
                {
                    Name = (string) ct["Name"],
                    CreatedAt = DateTimeOffset.Parse(createdAt),
                    CreatedBy = Guid.Parse((string) ct["CreatedBy"]),
                    Deleted = false
                };
            });
        }

        private IEnumerable<Subsystem> ConvertSubsystems(JArray subsystems)
        {
            return subsystems.Select(s => new Subsystem
            {
                Id = Convert.ToInt64((string) s["ID"]),
                Name = (string) s["Name"]
            });
        }

        private IEnumerable<Parameter> ConvertParameters(JArray json)
        {
            var pars = json.Select(j => new Parameter
            {
                Id = Convert.ToInt64((string) j["ID"]),
                Name = (string) j["Name"],
                Unit = (string) j["Unit"]
            });

            return pars;
        }

        private async Task<JArray> GetAndParseJson(string url, HttpClient client)
        {
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(content);
            return json;
        }
    }
}