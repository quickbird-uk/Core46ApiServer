using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Qb.Core46Api.Models;

namespace Qb.Core46Api.Migrations
{
    [DbContext(typeof(QbDbContext))]
    [Migration("20160821151531_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("OpenIddict.OpenIddictApplication", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ClientId");

                    b.Property<string>("ClientSecret");

                    b.Property<string>("DisplayName");

                    b.Property<string>("LogoutRedirectUri");

                    b.Property<string>("RedirectUri");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("OpenIddictApplications");
                });

            modelBuilder.Entity("OpenIddict.OpenIddictAuthorization", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Scope");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("OpenIddictAuthorizations");
                });

            modelBuilder.Entity("OpenIddict.OpenIddictScope", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Description");

                    b.HasKey("Id");

                    b.ToTable("OpenIddictScopes");
                });

            modelBuilder.Entity("OpenIddict.OpenIddictToken", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ApplicationId");

                    b.Property<string>("AuthorizationId");

                    b.Property<string>("Type");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("AuthorizationId");

                    b.HasIndex("UserId");

                    b.ToTable("OpenIddictTokens");
                });

            modelBuilder.Entity("Qb.Core46Api.Models.QbUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Qb.Poco.Global.CropType", b =>
                {
                    b.Property<string>("Name");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<Guid?>("CreatedBy");

                    b.Property<bool>("Deleted");

                    b.HasKey("Name");

                    b.ToTable("CropTypes");
                });

            modelBuilder.Entity("Qb.Poco.Global.Parameter", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Unit");

                    b.HasKey("Id");

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("Qb.Poco.Global.Placement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Placements");
                });

            modelBuilder.Entity("Qb.Poco.Global.SensorType", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ParameterId");

                    b.Property<long>("PlacementId");

                    b.Property<long>("SubsystemId");

                    b.HasKey("Id");

                    b.HasIndex("ParameterId");

                    b.HasIndex("PlacementId");

                    b.HasIndex("SubsystemId");

                    b.ToTable("SensorTypes");
                });

            modelBuilder.Entity("Qb.Poco.Global.Subsystem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Subsystems");
                });

            modelBuilder.Entity("Qb.Poco.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("IdentityId");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Qb.Poco.User.CropCycle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("CropTypeName");

                    b.Property<bool>("Deleted");

                    b.Property<DateTimeOffset?>("EndDate");

                    b.Property<Guid>("LocationId");

                    b.Property<string>("Name");

                    b.Property<DateTimeOffset>("StartDate");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<double>("Yield");

                    b.HasKey("Id");

                    b.HasIndex("CropTypeName");

                    b.HasIndex("LocationId");

                    b.ToTable("CropCycles");
                });

            modelBuilder.Entity("Qb.Poco.User.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<bool>("Deleted");

                    b.Property<Guid>("LocationId");

                    b.Property<string>("Name");

                    b.Property<Guid>("SerialNumber");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Qb.Poco.User.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<bool>("Deleted");

                    b.Property<string>("Name");

                    b.Property<Guid?>("PersonId");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Qb.Poco.User.Sensor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<bool>("Deleted");

                    b.Property<Guid>("DeviceId");

                    b.Property<bool>("Enabled");

                    b.Property<long>("SensorTypeId");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("SensorTypeId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("Qb.Poco.User.SensorHistory", b =>
                {
                    b.Property<Guid>("SensorId");

                    b.Property<DateTimeOffset>("TimeStamp");

                    b.Property<Guid?>("LocationId");

                    b.Property<byte[]>("RawData");

                    b.Property<DateTimeOffset>("UploadedAt");

                    b.HasKey("SensorId", "TimeStamp");

                    b.HasIndex("LocationId");

                    b.HasIndex("SensorId");

                    b.ToTable("SensorHistories");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Qb.Core46Api.Models.QbUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Qb.Core46Api.Models.QbUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Qb.Core46Api.Models.QbUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenIddict.OpenIddictAuthorization", b =>
                {
                    b.HasOne("Qb.Core46Api.Models.QbUser")
                        .WithMany("Authorizations")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("OpenIddict.OpenIddictToken", b =>
                {
                    b.HasOne("OpenIddict.OpenIddictApplication")
                        .WithMany("Tokens")
                        .HasForeignKey("ApplicationId");

                    b.HasOne("OpenIddict.OpenIddictAuthorization")
                        .WithMany("Tokens")
                        .HasForeignKey("AuthorizationId");

                    b.HasOne("Qb.Core46Api.Models.QbUser")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Qb.Poco.Global.SensorType", b =>
                {
                    b.HasOne("Qb.Poco.Global.Parameter", "Parameter")
                        .WithMany("SensorTypes")
                        .HasForeignKey("ParameterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Qb.Poco.Global.Placement", "Placement")
                        .WithMany("SensorTypes")
                        .HasForeignKey("PlacementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Qb.Poco.Global.Subsystem", "Subsystem")
                        .WithMany("SensorTypes")
                        .HasForeignKey("SubsystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Qb.Poco.User.CropCycle", b =>
                {
                    b.HasOne("Qb.Poco.Global.CropType", "CropType")
                        .WithMany("CropCycles")
                        .HasForeignKey("CropTypeName");

                    b.HasOne("Qb.Poco.User.Location", "Location")
                        .WithMany("CropCycles")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Qb.Poco.User.Device", b =>
                {
                    b.HasOne("Qb.Poco.User.Location", "Location")
                        .WithMany("Devices")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Qb.Poco.User.Location", b =>
                {
                    b.HasOne("Qb.Poco.Person", "Person")
                        .WithMany("Locations")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Qb.Poco.User.Sensor", b =>
                {
                    b.HasOne("Qb.Poco.User.Device", "Device")
                        .WithMany("Sensors")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Qb.Poco.Global.SensorType", "SensorType")
                        .WithMany()
                        .HasForeignKey("SensorTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Qb.Poco.User.SensorHistory", b =>
                {
                    b.HasOne("Qb.Poco.User.Location", "Location")
                        .WithMany("SensorHistories")
                        .HasForeignKey("LocationId");

                    b.HasOne("Qb.Poco.User.Sensor", "Sensor")
                        .WithMany("SensorHistories")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
