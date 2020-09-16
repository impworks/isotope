using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

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
    }
}