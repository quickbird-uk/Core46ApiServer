using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenIddict;
using Qb.Poco;
using Qb.Poco.Global;
using Qb.Poco.Hardware;
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
            builder.Entity<Person>().HasKey(p => p.Id);
            builder.Entity<Person>().Property(p => p.Id).ValueGeneratedNever();

            builder.Entity<SensorHistory>().HasKey(sd => new {sd.SensorId, sd.TimeStamp});

            builder.Entity<Location>().Property(gh => gh.Person).IsRequired();
            builder.Entity<Location>()
                .HasOne(gh => gh.Person)
                .WithMany(p => p.Locations)
                .HasForeignKey(gh => gh.PersonId)
                .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<Location>()
                .HasMany(gh => gh.SensorHistories)
                .WithOne(sd => sd.Location)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            //Add index on cropType Name and make it non Db generated
            builder.Entity<CropType>().HasKey(ct => ct.Name);
            builder.Entity<CropType>().Property(ct => ct.Name).ValueGeneratedNever();

            builder.Entity<CropType>()
                .HasMany(ct => ct.CropCycles)
                .WithOne(cc => cc.CropType)
                .HasForeignKey(cc => cc.CropTypeName);

            builder.Entity<SensorHistory>().HasIndex(sh => sh.UploadedAt);

            base.OnModelCreating(builder);
        }
    }
}