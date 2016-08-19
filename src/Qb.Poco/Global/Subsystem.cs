using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qb.Poco.Global
{
    public class Subsystem : IHasId
    {
        public string Name { get; set; }

        [JsonIgnore]
        public virtual List<RelayType> ControlTypes { get; set; }

        [JsonIgnore]
        public virtual List<SensorType> SensorTypes { get; set; }

        public long ID { get; set; }
    }
}