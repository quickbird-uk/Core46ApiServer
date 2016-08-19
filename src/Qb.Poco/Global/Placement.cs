using System.Collections.Generic;
using Newtonsoft.Json;
using Qb.Poco.Hardware;

namespace Qb.Poco.Global
{
    public class Placement : IHasId
    {
        public string Name { get; set; }

        [JsonIgnore]
        public virtual List<Sensor> Sensors { get; set; }

        public long ID { get; set; }
    }
}