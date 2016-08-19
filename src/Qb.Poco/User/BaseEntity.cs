using System;

namespace Qb.Poco.User
{
    public class BaseEntity : IHasGuid
    {
        /// <summary>Don't change this after the object's creation</summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        /// <summary>Should be updated every time a value is locally changed.</summary>
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

        /// <summary>used for soft-delete. As a rule of thumb, don't display these items in UI</summary>
        public bool Deleted { get; set; } = false;

        public Guid ID { get; set; }
    }
}