using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qb.Poco.User
{
    public class Person : IHasGuid
    {
        public ulong twitterID { get; set; }

        public string TwitterHandle { get; set; }

        public string UserName { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

        [JsonIgnore]
        public virtual List<Location> Locations { get; set; }

        public Guid ID { get; set; }
    }
}