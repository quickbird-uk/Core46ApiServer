using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Qb.Poco.User;

namespace Qb.Poco.Hardware
{
    public class Device : BaseEntity
    {
        public string Name { get; set; }

        public Guid SerialNumber { get; set; }

        /// <remarks>fk-nav</remarks>
        [JsonIgnore]
        public virtual Location Location { get; set; }

        /// <remarks>fk</remarks>
        public Guid LocationID { get; set; }

        /// <remarks>nav</remarks>
        [JsonIgnore]
        public virtual List<Sensor> Sensors { get; set; }
    }
}