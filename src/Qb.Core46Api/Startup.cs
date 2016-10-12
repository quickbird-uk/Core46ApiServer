using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Qb.Core46Api.Models;
using Qb.Core46Api.Services;
using Qb.Core46Api.Vars;

namespace Qb.Core46Api
{
    public class Startup
    {
        private readonly bool _dev;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsEnvironment("Development"))
            {
                _dev = true;
                _dev = true;
                builder.AddApplicationInsightsSettings(true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            // Add the database context, defaults to scoped; new context for each request.
            services.AddDbContext<QbDbContext>(
                options => { options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); });

            // Adds Identity to IoC and confugres with the database.
            services.AddIdentity<QbUser, IdentityRole>(ConfigureIdentityOptions)
                .AddEntityFrameworkStores<QbDbContext>()
                .AddDefaultTokenProviders();

            var openIddict = services.AddOpenIddict<QbUser, QbDbContext>()
                .EnableTokenEndpoint("/api/auth/token") // Password grant route
                .AllowPasswordFlow() // Enables password grant
                .AddSigningKey(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("A rather basic key")))
                .SetAccessTokenLifetime(TimeSpan.FromDays(365));

            if (_dev)
                openIddict.DisableHttpsRequirement();

            services.AddSwaggerGen();

            services.Configure<SmsSenderOptions>(Configuration.GetSection("Twilio"));
            services.AddTransient<ISmsSender, SmsSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory)
        {
            // Create a closed DI scope to run initial configuration.
            using (var scope = scopeFactory.CreateScope())
            {
                ConfigureDbAdminAndRoles(scope).Wait();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
                app.UseExceptionHandler("/error.html");

            app.UseIdentity(); // Authorization using ASP Identity.
            app.UseOAuthValidation();
            app.UseOpenIddict(); // OpenIddict takes care of the token issuing.

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();

            // Simple universal 404.
            app.Run(async context =>
            {
                context.Response.ContentType = "text/html"; //Edge becomes unprefictable if you dont set this.
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("\uFEFF404"); //First character is BOM for UTF-8.
            });
        }


        private static void ConfigureIdentityOptions(IdentityOptions options)
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        }

        /// <summary>Initialises admin user and roles.</summary>
        /// <param name="scope">A limited DI Scope.</param>
        private async Task ConfigureDbAdminAndRoles(IServiceScope scope)
        {
            // Don't db.Database.Migrate here, slows server start time, do it on publish instead.

            var roleMan = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            if (!roleMan.Roles.Any(r => r.Name == Roles.Admin))
                await roleMan.CreateAsync(new IdentityRole(Roles.Admin));

            if (!roleMan.Roles.Any(r => r.Name == Roles.EditGlobalData))
                await roleMan.CreateAsync(new IdentityRole(Roles.EditGlobalData));

            if (!roleMan.Roles.Any(r => r.Name == Roles.EditUserData))
                await roleMan.CreateAsync(new IdentityRole(Roles.EditUserData));

            var userManager = scope.ServiceProvider.GetService<UserManager<QbUser>>();
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var adminUser = new QbUser {UserName = "admin", PhoneNumberConfirmed = true};
                await userManager.CreateAsync(adminUser, "xxxxxxxx");
                adminUser = await userManager.FindByNameAsync("admin");
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
        }
    }
}