using System;
using System.IO;
using System.Threading.Tasks;
using Isotope.Code.Services.Config;
using Isotope.Data;
using Isotope.Data.Models;
using Isotope.Data.Utils;
using Isotope.Demo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Directory = System.IO.Directory;

namespace Isotope.Code.Config;

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
        CreateStorageFolder();

        var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var sp = scope.ServiceProvider;
        InitDatabaseAsync(sp).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Performs database seeding.
    /// </summary>
    private async Task InitDatabaseAsync(IServiceProvider sp)
    {
        var demoCfg = Configuration.DemoMode ?? new DemoModeConfig(); // all false

        if (demoCfg.Enabled && demoCfg.ClearOnStartup)
            await SeedData.ClearPreviousDataAsync();

        var db = sp.GetService<AppDbContext>();
        var userMgr = sp.GetService<UserManager<AppUser>>();

        await db.EnsureDatabaseCreatedAsync();
        await db.EnsureSystemItemsCreatedAsync(userMgr, demoCfg);

        if (demoCfg.Enabled && demoCfg.SeedSampleData)
            await SeedData.SeedSampleDataAsync(db);
    }

    /// <summary>
    /// Creates the folder for database & keys.
    /// </summary>
    private void CreateStorageFolder()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "@media");
        Directory.CreateDirectory(path);
    }
}