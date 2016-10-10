namespace Qb.Poco.User
{
    /// <summary>All the sync data types (expluding SensorHistory) together for serialization.</summary>
    /// <remarks>Not a db class.</remarks>
    public class SyncData
    {
        public SyncData(long serverDateTime, Person person, Location[] locations, Device[] devices,
            CropCycle[] cropCycles, Sensor[] sensors)
        {
            ServerDateTime = serverDateTime;
            Person = person;
            Locations = locations;
            Devices = devices;
            CropCycles = cropCycles;
            Sensors = sensors;
        }

        public long ServerDateTime { get; }
        public Person Person{ get; }
        public Location[] Locations { get; }
        public Device[] Devices { get; }
        public CropCycle[] CropCycles { get; }
        public Sensor[] Sensors { get; }
    }
}