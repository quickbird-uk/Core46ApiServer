using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qb.Poco.User
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }

        [JsonIgnore]
        public virtual Person Person { get; set; }

        public Guid PersonId { get; set; }

        [JsonIgnore]
        public virtual List<Device> Devices { get; set; }

        [JsonIgnore]
        public virtual List<CropCycle> CropCycles { get; set; }

        [JsonIgnore]
        public virtual List<SensorHistory> SensorHistory { get; set; }

        [JsonIgnore]
        public virtual List<RelayHistory> RelayHistory { get; set; }
    }
}