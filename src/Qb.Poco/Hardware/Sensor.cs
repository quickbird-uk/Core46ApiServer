using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Qb.Poco.Global;
using Qb.Poco.User;

namespace Qb.Poco.Hardware
{
    public class Sensor : BaseEntity
    {
        public bool Enabled { get; set; } = false;

        /// <remarks>Must not be changed</remarks>
        [JsonIgnore]
        public virtual SensorType SensorType { get; set; }

        public long SensorTypeID { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }

        public Guid DeviceID { get; set; }

        [JsonIgnore]
        public virtual List<SensorHistory> SensorHistory { get; set; }
    }
}