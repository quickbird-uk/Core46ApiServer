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
        public DbSet<SensorType> SensorTypes { get; set; }
        public DbSet<Subsystem> Subsystems { get; set; }

        public DbSet<Person> People { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<CropCycle> CropCycles { get; set; }
        public DbSet<CropType> CropTypes { get; set; }
        public DbSet<SensorHistory> SensorHistories { get; set; }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Sensor> Sensors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SensorHistory>().HasKey(sd => new {sd.SensorID, sd.TimeStamp});

            builder.Entity<Location>().Property(gh => gh.Person).IsRequired();
            builder.Entity<Location>()
                .HasOne(gh => gh.Person)
                .WithMany(p => p.Locations)
                .HasForeignKey(gh => gh.PersonId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Person>().HasKey(p => p.ID);
            builder.Entity<Person>().Property(p => p.ID).ValueGeneratedNever();

            builder.Entity<Location>()
                .HasMany(gh => gh.SensorHistory)
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

        }
    }
}