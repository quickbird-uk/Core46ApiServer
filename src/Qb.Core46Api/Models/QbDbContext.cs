using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenIddict;
using Qb.Poco;
using Qb.Poco.Global;
using Qb.Poco.User;

namespace Qb.Core46Api.Models
{
    public class QbDbContext : OpenIddictDbContext<QbUser>
    {
        public QbDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Placement> Placements { get; set; }
        public DbSet<Subsystem> Subsystems { get; set; }
        public DbSet<CropType> CropTypes { get; set; }

        /// <summary>Depends on Parameters, Placements and Subsystems.</summary>
        public DbSet<SensorType> SensorTypes { get; set; }


        public DbSet<Person> People { get; set; }

        /// <summary>Depends on People.</summary>
        public DbSet<Location> Locations { get; set; }

        /// <summary>Depends on Locations.</summary>
        public DbSet<CropCycle> CropCycles { get; set; }

        /// <summary>Depends on Locations.</summary>
        public DbSet<Device> Devices { get; set; }

        /// <summary>Depends on Devices and SensorTypes.</summary>
        public DbSet<Sensor> Sensors { get; set; }

        /// <summary>Depends on Locations and Sensors.</summary>
        public DbSet<SensorHistory> SensorHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Only entity with pk that is not ID.
            builder.Entity<CropType>().HasKey(ct => ct.Name);

            // A weak unenforced foreign key to Person.
            builder.Entity<CropType>().Property(ct => ct.CreatedBy).IsRequired(false);

            // Composite key.
            builder.Entity<SensorHistory>().HasKey(sd => new {sd.SensorId, sd.UtcDate});

            // Set optional foreign key, defaults delete to restrict.
            builder.Entity<Location>().Property(loc => loc.PersonId).IsRequired(false);
            // Changed behaviour to setnull, allows remaking people without hurtning data.
            builder.Entity<Location>()
                .HasOne(loc => loc.Person)
                .WithMany(p => p.Locations)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            UpdateUpdateAtOnBaseEntity();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateUpdateAtOnBaseEntity();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateUpdateAtOnBaseEntity()
        {
            var entities = ChangeTracker.Entries()
                .Where(
                    e => e.Entity is BaseEntity && ((e.State == EntityState.Added) || (e.State == EntityState.Modified)));
            var time = DateTimeOffset.UtcNow;
            foreach (var entity in entities)
            {
                var baseEntity = (BaseEntity) entity.Entity;
                baseEntity.UpdatedAt = time;
            }
        }
    }
}