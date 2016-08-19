using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Qb.Poco.User;

namespace Qb.Poco.Global
{
    public class CropType
    {
        public string Name { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public bool Approved { get; set; } = false;

        public Guid? CreatedBy { get; set; } = null;

        [JsonIgnore]
        public virtual List<CropCycle> CropCycles { get; set; }
    }
}