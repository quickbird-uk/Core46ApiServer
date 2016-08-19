using Newtonsoft.Json;

namespace Qb.Poco.Global
{
    public class SensorType : IHasId
    {
        /// <remarks>fk-nav</remarks>
        [JsonIgnore]
        public virtual Subsystem Subsystem { get; set; }

        /// <remarks>fk</remarks>
        public long SubsystemID { get; set; }

        /// <remarks>fk-nav</remarks>
        [JsonIgnore]
        public virtual Placement Place { get; set; }

        /// <remarks>fk</remarks>
        public long PlaceID { get; set; }

        /// <remarks>fk-nav</remarks>
        [JsonIgnore]
        public virtual Parameter Param { get; set; }

        /// <remarks>fk</remarks>
        public long ParamID { get; set; }

        public long ID { get; set; }
    }
}