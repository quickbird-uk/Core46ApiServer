using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qb.Poco.User
{
    public class RelayHistory : IValidatableObject
    {
        [JsonIgnore]
        public virtual Relay Relay { get; set; }

        // TODO: This need to be the same name (for an interface) and SensorHistory.SensorID
        [Required]
        public Guid RelayID { get; set; }

        [JsonIgnore]
        public virtual Location Location { get; set; }

        public Guid? LocationID { get; set; }

        [Required]
        public DateTimeOffset TimeStamp { get; set; }

        [JsonIgnore]
        public byte[] RawData { get; set; } = new byte[0];

        //EDIT EF code to make this NOT mapped to a table! Otherwise we will have trouble! 
        //this is used for network communication and by the program at runtime! 
        [Required]
        public List<RelayDatapoint> Data { get; set; }


        public void SerialiseData()
        {
            var dataSize = RelayDatapoint.BinarySize;
            var dataRaw = new byte[Data.Count*dataSize];

            for (var i = 0; i < Data.Count; i++)
            {
                dataRaw[i*dataSize] = Data[i].State ? (byte) 1 : (byte) 0;
                var durationBytes = BitConverter.GetBytes(Data[i].Duration.Ticks);
                Array.Copy(durationBytes, 0, dataRaw, i*dataSize + 1, 8);
                var ticks = Data[i].TimeStamp.UtcTicks;
                var timestampBytes = BitConverter.GetBytes(ticks);
                Array.Copy(timestampBytes, 0, dataRaw, i*dataSize + 9, 8);
            }
            RawData = dataRaw;
        }

        public void DeserialiseData()
        {
            var dataSize = RelayDatapoint.BinarySize;
            var dataItems = new List<RelayDatapoint>();

            for (var i = 0; i < RawData.Length; i += dataSize)
            {
                var state = RawData[i] > 0;
                var duration = TimeSpan.FromTicks(BitConverter.ToInt64(RawData, i + 1));
                var timestampTicks = BitConverter.ToInt64(RawData, i + 9);
                var timeStamp = new DateTimeOffset(timestampTicks, TimeStamp.Offset).Add(TimeStamp.Offset);
                ;
                dataItems.Add(new RelayDatapoint(state, timeStamp, duration));
            }

            Data = dataItems;
        }

        /// <summary>Creates a new controlHistory object that contains only data past a certain point. This is done based on the
        ///     list, not Raw data. If the list is empty, you will get nothing!</summary>
        /// <param name="slicePoint">Time, before which data is not included</param>
        /// <returns></returns>
        public RelayHistory Slice(DateTimeOffset slicePoint)
        {
            var result = new RelayHistory
            {
                Relay = Relay,
                RelayID = RelayID,
                TimeStamp = TimeStamp,
                LocationID = LocationID,
                Location = Location,
                Data = new List<RelayDatapoint>()
            };
            foreach (var item in Data)
                if (item.TimeStamp > slicePoint)
                    result.Data.Add(item);
            return result;
        }

        /// <summary>Will only merge the two if they have same RelayID and same Timestamp. Mergning is done based on the list, not
        ///     raw data.</summary>
        /// <param name="mergeWith"></param>
        /// <returns></returns>
        public static RelayHistory Merge(RelayHistory slice1, RelayHistory slice2)
        {
            if (slice1.RelayID != slice2.RelayID)
                throw new Exception("Attempted to merge RelayHistory slices with different ID's! "
                                    + slice1.RelayID + " and " + slice2.RelayID);
            if (slice1.LocationID != slice2.LocationID)
                throw new Exception("Attempted to merge RelayHistory from different Locations!! "
                                    + slice1.LocationID + " and " + slice2.LocationID);
            if (((slice1.TimeStamp - slice2.TimeStamp).TotalHours > 24)
                || ((slice2.TimeStamp - slice1.TimeStamp).TotalHours > 24))
                throw new Exception("Attempted to merge RelayHistory from different days! "
                                    + slice1.TimeStamp + " and " + slice2.TimeStamp);

            var result = new RelayHistory
            {
                RelayID = slice1.RelayID,
                Relay = slice1.Relay,
                TimeStamp = slice1.TimeStamp,
                LocationID = slice1.LocationID,
                Location = slice1.Location,
                Data = new List<RelayDatapoint>()
            };

            var tempList = new List<RelayDatapoint>(slice1.Data);
            tempList.AddRange(slice2.Data);

            tempList.Sort((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));

            for (var i = 0; i < tempList.Count; i++)
                if (i >= tempList.Count - 1)
                    result.Data.Add(tempList[i]);
                else if (tempList[i].TimeStamp != tempList[i + 1].TimeStamp)
                    result.Data.Add(tempList[i]);
            return result;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (TimeStamp == null)
            //{
            //    yield return new ValidationResult("You need to provide a timestamp");
            //};
            if (TimeStamp.TimeOfDay != TimeSpan.Zero)
                yield return new ValidationResult("Timestamp on the Day should be zero!");
            ;
            if (Data.Exists(dt => dt.TimeStamp > TimeStamp))
                yield return new ValidationResult("Timestamps of measurements must be earlier than the day's Timestamp")
                    ;
            ;
            if (Data.Exists(dt => (TimeStamp - dt.TimeStamp).TotalHours > 24))
                yield return new ValidationResult("Timestamps should be taken withing 24 hours of the day.");
            ;
            if (Data.Exists(dt => dt.TimeStamp > DateTimeOffset.Now))
                yield return new ValidationResult("You can't create a datapoint in the future");
            ;
            if (TimeStamp > DateTimeOffset.Now.AddDays(1))
                yield return new ValidationResult("You can't create a day that's more than 24 hours in the future");
            ;
        }
    }

    public class RelayDatapoint
    {
        /// <summary>Size of these structures in bytes</summary>
        public static readonly int BinarySize = 17;

        [Required] public readonly TimeSpan Duration;

        [Required] public readonly bool State;

        [Required] public readonly DateTimeOffset TimeStamp;

        public RelayDatapoint(bool state, DateTimeOffset timestamp, TimeSpan duration)
        {
            State = state;
            TimeStamp = timestamp;
            Duration = duration;
        }
    }
}