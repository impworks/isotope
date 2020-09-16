using System.IO;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Isotope.Data.Utils
{
    /// <summary>
    /// Configures the design-time instance of the data context (for migrations).
    /// </summary>
    [UsedImplicitly]
    public class MigrationConfigurator: IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json")
                         .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlite(config["Database:ConnectionString"]);

            return new AppDbContext(builder.Options);
        }
    }
}