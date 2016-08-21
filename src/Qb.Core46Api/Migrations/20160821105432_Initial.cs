using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Qb.Core46Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AspNetRoles",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints:
                table => { table.PrimaryKey("PK_AspNetUserTokens", x => new {x.UserId, x.LoginProvider, x.Name}); });

            migrationBuilder.CreateTable(
                "OpenIddictApplications",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    ClientSecret = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    LogoutRedirectUri = table.Column<string>(nullable: true),
                    RedirectUri = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_OpenIddictApplications", x => x.Id); });

            migrationBuilder.CreateTable(
                "OpenIddictScopes",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_OpenIddictScopes", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetUsers",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                "CropTypes",
                table => new
                {
                    Name = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_CropTypes", x => x.Name); });

            migrationBuilder.CreateTable(
                "Parameters",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Parameters", x => x.Id); });

            migrationBuilder.CreateTable(
                "Placements",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Placements", x => x.Id); });

            migrationBuilder.CreateTable(
                "Subsystems",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Subsystems", x => x.Id); });

            migrationBuilder.CreateTable(
                "People",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    IdentityId = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_People", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetUserClaims_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey(
                        "FK_AspNetUserLogins_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "OpenIddictAuthorizations",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Scope = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                    table.ForeignKey(
                        "FK_OpenIddictAuthorizations_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "SensorTypes",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParameterId = table.Column<long>(nullable: false),
                    PlacementId = table.Column<long>(nullable: false),
                    SubsystemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorTypes", x => x.Id);
                    table.ForeignKey(
                        "FK_SensorTypes_Parameters_ParameterId",
                        x => x.ParameterId,
                        "Parameters",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_SensorTypes_Placements_PlacementId",
                        x => x.PlacementId,
                        "Placements",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_SensorTypes_Subsystems_SubsystemId",
                        x => x.SubsystemId,
                        "Subsystems",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Locations",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PersonId = table.Column<Guid>(nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        "FK_Locations_People_PersonId",
                        x => x.PersonId,
                        "People",
                        "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                "OpenIddictTokens",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ApplicationId = table.Column<string>(nullable: true),
                    AuthorizationId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                    table.ForeignKey(
                        "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                        x => x.ApplicationId,
                        "OpenIddictApplications",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                        x => x.AuthorizationId,
                        "OpenIddictAuthorizations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_OpenIddictTokens_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "CropCycles",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    CropTypeName = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Yield = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropCycles", x => x.Id);
                    table.ForeignKey(
                        "FK_CropCycles_CropTypes_CropTypeName",
                        x => x.CropTypeName,
                        "CropTypes",
                        "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_CropCycles_Locations_LocationId",
                        x => x.LocationId,
                        "Locations",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Devices",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        "FK_Devices_Locations_LocationId",
                        x => x.LocationId,
                        "Locations",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Sensors",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DeviceId = table.Column<Guid>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    SensorTypeId = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                    table.ForeignKey(
                        "FK_Sensors_Devices_DeviceId",
                        x => x.DeviceId,
                        "Devices",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Sensors_SensorTypes_SensorTypeId",
                        x => x.SensorTypeId,
                        "SensorTypes",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "SensorHistories",
                table => new
                {
                    SensorId = table.Column<Guid>(nullable: false),
                    TimeStamp = table.Column<DateTimeOffset>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: true),
                    RawData = table.Column<byte[]>(nullable: true),
                    UploadedAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorHistories", x => new {x.SensorId, x.TimeStamp});
                    table.ForeignKey(
                        "FK_SensorHistories_Locations_LocationId",
                        x => x.LocationId,
                        "Locations",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_SensorHistories_Sensors_SensorId",
                        x => x.SensorId,
                        "Sensors",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName");

            migrationBuilder.CreateIndex(
                "IX_AspNetRoleClaims_RoleId",
                "AspNetRoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserClaims_UserId",
                "AspNetUserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserLogins_UserId",
                "AspNetUserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_RoleId",
                "AspNetUserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_UserId",
                "AspNetUserRoles",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_OpenIddictApplications_ClientId",
                "OpenIddictApplications",
                "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_OpenIddictAuthorizations_UserId",
                "OpenIddictAuthorizations",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_OpenIddictTokens_ApplicationId",
                "OpenIddictTokens",
                "ApplicationId");

            migrationBuilder.CreateIndex(
                "IX_OpenIddictTokens_AuthorizationId",
                "OpenIddictTokens",
                "AuthorizationId");

            migrationBuilder.CreateIndex(
                "IX_OpenIddictTokens_UserId",
                "OpenIddictTokens",
                "UserId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "AspNetUsers",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_SensorTypes_ParameterId",
                "SensorTypes",
                "ParameterId");

            migrationBuilder.CreateIndex(
                "IX_SensorTypes_PlacementId",
                "SensorTypes",
                "PlacementId");

            migrationBuilder.CreateIndex(
                "IX_SensorTypes_SubsystemId",
                "SensorTypes",
                "SubsystemId");

            migrationBuilder.CreateIndex(
                "IX_CropCycles_CropTypeName",
                "CropCycles",
                "CropTypeName");

            migrationBuilder.CreateIndex(
                "IX_CropCycles_LocationId",
                "CropCycles",
                "LocationId");

            migrationBuilder.CreateIndex(
                "IX_Devices_LocationId",
                "Devices",
                "LocationId");

            migrationBuilder.CreateIndex(
                "IX_Locations_PersonId",
                "Locations",
                "PersonId");

            migrationBuilder.CreateIndex(
                "IX_Sensors_DeviceId",
                "Sensors",
                "DeviceId");

            migrationBuilder.CreateIndex(
                "IX_Sensors_SensorTypeId",
                "Sensors",
                "SensorTypeId");

            migrationBuilder.CreateIndex(
                "IX_SensorHistories_LocationId",
                "SensorHistories",
                "LocationId");

            migrationBuilder.CreateIndex(
                "IX_SensorHistories_SensorId",
                "SensorHistories",
                "SensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "AspNetRoleClaims");

            migrationBuilder.DropTable(
                "AspNetUserClaims");

            migrationBuilder.DropTable(
                "AspNetUserLogins");

            migrationBuilder.DropTable(
                "AspNetUserRoles");

            migrationBuilder.DropTable(
                "AspNetUserTokens");

            migrationBuilder.DropTable(
                "OpenIddictScopes");

            migrationBuilder.DropTable(
                "OpenIddictTokens");

            migrationBuilder.DropTable(
                "CropCycles");

            migrationBuilder.DropTable(
                "SensorHistories");

            migrationBuilder.DropTable(
                "AspNetRoles");

            migrationBuilder.DropTable(
                "OpenIddictApplications");

            migrationBuilder.DropTable(
                "OpenIddictAuthorizations");

            migrationBuilder.DropTable(
                "CropTypes");

            migrationBuilder.DropTable(
                "Sensors");

            migrationBuilder.DropTable(
                "AspNetUsers");

            migrationBuilder.DropTable(
                "Devices");

            migrationBuilder.DropTable(
                "SensorTypes");

            migrationBuilder.DropTable(
                "Locations");

            migrationBuilder.DropTable(
                "Parameters");

            migrationBuilder.DropTable(
                "Placements");

            migrationBuilder.DropTable(
                "Subsystems");

            migrationBuilder.DropTable(
                "People");
        }
    }
}