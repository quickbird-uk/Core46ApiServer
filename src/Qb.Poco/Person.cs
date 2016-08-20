using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Qb.Poco.User;

namespace Qb.Poco
{
    public class Person : IHasGuid
    {
        /// <summary>The Id of the linked Identity (soft linked for flexibility).</summary>
        public string IdentityId { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

        /// <remarks>nav</remarks>
        [JsonIgnore]
        public virtual List<Location> Locations { get; set; }

        public Guid Id { get; set; }
    }
}