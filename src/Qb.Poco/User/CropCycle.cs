using System;
using Qb.Poco.Global;

namespace Qb.Poco.User
{
    public class CropCycle : BaseEntity
    {
        public string Name { get; set; }

        public double Yield { get; set; } = 0;

        public string CropVariety { get; set; } = "Unknown";

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public virtual CropType CropType { get; set; }

        public string CropTypeName { get; set; }

        public virtual Location Location { get; set; }

        public Guid LocationID { get; set; }
    }
}