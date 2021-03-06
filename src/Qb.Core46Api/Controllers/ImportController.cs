﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


                await _db.CropTypes.AddOrUpdateRange(
                    ConvertCropTypes(croptypes), (p1, p2) => p1.Name == p2.Name);
                // This save doesn't require IdentityInsert, gets bundled into the next save.
                await _db.Parameters.AddOrUpdateRange(
                    ConvertParameters(parameters), (p1, p2) => p1.Id == p2.Id);
                SaveIdentityInsert("Parameters", _db);

                await _db.Placements.AddOrUpdateRange(
                    ConvertPlacements(placements), (p1, p2) => p1.Id == p2.Id);
                SaveIdentityInsert("Placements", _db);

                await _db.Subsystems.AddOrUpdateRange(
                    ConvertSubsystems(subsystems), (p1, p2) => p1.Id == p2.Id);
                SaveIdentityInsert("Subsystems", _db);

                await _db.SensorTypes.AddOrUpdateRange(
                    ConvertSensorTypes(sensorTypes), (p1, p2) => p1.Id == p2.Id);
                SaveIdentityInsert("SensorTypes", _db);

                //await _db.SaveChangesAsync();

                return Content($"[{parameters},{subsystems},{croptypes},{sensorTypes},{placements}]");
            }
        }


        private void SaveIdentityInsert(string name, DbContext db)
        {
            db.Database.OpenConnection();
            try
            {
                db.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT dbo.{name} ON");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand($"SET IDENTITY_INSERT dbo.{name} OFF");
            }
            finally
            {
                db.Database.CloseConnection();
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
            return croptypes.Select(ct => new CropType
            {
                Name = (string) ct["Name"],
                CreatedAt = ParseUsDate((string) ct["CreatedAt"]),
                CreatedBy = Guid.Parse((string) ct["CreatedBy"]),
                Deleted = false
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

        private DateTimeOffset ParseUsDate(string date)
        {
            return DateTimeOffset.ParseExact(date, @"MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}