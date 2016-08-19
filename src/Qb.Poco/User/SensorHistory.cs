using System;
using Newtonsoft.Json;
using Qb.Poco.Hardware;

namespace Qb.Poco.User
{
    public class SensorHistory
    {
        [JsonIgnore]
        public virtual Location Location { get; set; }

        public Guid? LocationID { get; set; }

        [JsonIgnore]
        public byte[] RawData { get; set; } = new byte[0];

        [JsonIgnore]
        public virtual Sensor Sensor { get; set; }

        public Guid SensorID { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>The datetime that this history was uploaded to the server, always set by the server and only by the server.
        ///     For the local computer this is a gaurantee that data up to this point has been downloaded from the server.</summary>
        public DateTimeOffset UploadedAt { get; set; } = default(DateTimeOffset);
    }


    public class SensorDatapoint
    {
        /// <summary>Size of these structures in bytes</summary>
        public static readonly int BinarySize = 24;

        public readonly TimeSpan Duration;

        public readonly DateTimeOffset TimeStamp;

        public readonly double Value;

        //public override bool Equals(object obj)
        //{
        //    SensorDatapoint comparand = obj as SensorDatapoint;
        //    if (comparand == null)
        //        return false;
        //    else
        //    return base.Equals(obj);
        //}

        public SensorDatapoint(double value, DateTimeOffset timestamp, TimeSpan duration)
        {
            Value = value;
            TimeStamp = timestamp;
            Duration = duration;
        }
    }
}