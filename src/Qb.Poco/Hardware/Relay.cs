using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Qb.Poco.Global;
using Qb.Poco.User;

namespace Qb.Poco
{
    public class Relay : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int OnTime { get; set; }

        [Required]
        public int OffTime { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public bool Enabled { get; set; } = false;

        [JsonIgnore]
        public virtual RelayType RelayType { get; set; }

        [Required]
        public long RelayTypeID { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }

        [Required]
        public Guid DeviceID { get; set; }


        [JsonIgnore]
        public virtual List<RelayHistory> RelayHistory { get; set; }
    }
}