using System.Threading.Tasks;
using Isotope.Code.Services.Config;
using Isotope.Data;
using Isotope.Data.Models;
using Isotope.Data.Seed;
using Isotope.Data.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Isotope.Code.Config
{
    public partial class Startup
    {
        /// <summary>
        /// Registers database-related services.
        /// </summary>
        private void ConfigureDatabaseServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(Configuration.Database.ConnectionString));

            services.AddIdentity<AppUser, IdentityRole>(opts =>
                    {
                        opts.Password.RequiredLength = 6;
                        opts.Password.RequireDigit = false;
                        opts.Password.RequireLowercase = false;
                        opts.Password.RequireUppercase = false;
                        opts.Password.RequireNonAlphanumeric = false;
                        opts.Password.RequiredUniqueChars = 1;
                    })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Creates the database and applies migrations if necessary.
        /// </summary>
        private void InitDatabase(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var sp = scope.ServiceProvider;

            var db = sp.GetService<AppDbContext>();
            InitDatabaseAsync(db).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs database seeding.
        /// </summary>
        private async Task InitDatabaseAsync(AppDbContext db)
        {
            var demoCfg = Configuration.DemoMode ?? new DemoModeConfig(); // all false
            
            if (demoCfg.Enabled && demoCfg.ClearOnStartup)
                await SeedData.ClearPreviousData();
            
            await db.EnsureDatabaseCreatedAsync();
            await db.EnsureSystemItemsCreatedAsync();

            if (demoCfg.Enabled && demoCfg.SeedSampleData)
                await SeedData.SeedSampleDataAsync(db);
        }
    }
}