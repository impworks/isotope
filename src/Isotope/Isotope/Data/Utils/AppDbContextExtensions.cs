using System.Linq;
using System.Threading.Tasks;
using Impworks.Utils.Format;
using Isotope.Code.Services.Config;
using Isotope.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Isotope.Data.Utils
{
    /// <summary>
    /// Extensions methods for configuring and seeding the database data.
    /// </summary>
    public static class AppDbContextExtensions
    {
        /// <summary>
        /// Applies migrations and applies migrations.
        /// </summary>
        public static async Task EnsureDatabaseCreatedAsync(this AppDbContext context)
        {
            if(!await context.IsMigratedAsync())
                await context.Database.MigrateAsync();
        }

        /// <summary>
        /// Checks if there are no pending migrations.
        /// </summary>
        private static async Task<bool> IsMigratedAsync(this AppDbContext context)
        {
            var applied = await context.GetService<IHistoryRepository>().GetAppliedMigrationsAsync();
            var total = context.GetService<IMigrationsAssembly>().Migrations;
            return !total.Select(x => x.Key).Except(applied.Select(x => x.MigrationId)).Any();
        }

        /// <summary>
        /// Ensures all system entries are created in the DB.
        /// </summary>
        public static async Task EnsureSystemItemsCreatedAsync(this AppDbContext db)
        {
            if(!db.Roles.Any())
            {
                db.Roles.AddRange(
                    EnumHelper.GetEnumValues<UserRole>()
                              .Select(name => new IdentityRole { Name = name.ToString(), NormalizedName = name.ToString().ToUpper() })
                );
            }
            
            if (!db.DynamicConfig.Any())
            {
                db.DynamicConfig.Add(new DynamicConfigWrapper
                {
                    Id = 1,
                    Value = JsonConvert.SerializeObject(new DynamicConfig
                    {
                        Title = "Isotope",
                        AllowGuests = true
                    })
                });
            }

            await db.SaveChangesAsync();
        }
    }
}