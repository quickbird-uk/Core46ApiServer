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

        [JsonIgnore]
        public virtual Location Location { get; set; }

        public Guid LocationID { get; set; }

        [JsonIgnore]
        public virtual List<Sensor> Sensors { get; set; }
    }
}