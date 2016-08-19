using Newtonsoft.Json;

namespace Qb.Poco.Global
{
    public class RelayType : IHasId
    {
        public string Name { get; set; }

        public bool Additive { get; set; }

        [JsonIgnore]
        public virtual Subsystem Subsystem { get; set; }

        public long SubsystemID { get; set; }
        public long ID { get; set; }
    }
}