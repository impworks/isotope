using Isotope.Data;
using Isotope.Data.Models;
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
        /// <param name="services"></param>
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
            db.EnsureDatabaseCreatedAsync().GetAwaiter().GetResult();
            
            // todo: seed default data!
        }
    }
}