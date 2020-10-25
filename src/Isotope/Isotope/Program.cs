using System;
using System.IO;
using System.Reflection;
using Isotope.Areas.Admin.Services.Jobs;
using Isotope.Code.Config;
using Isotope.Code.Services.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Isotope
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Microsoft.AspNetCore.Routing.UseCorrectCatchAllBehavior", true);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.UseKestrel(x => x.ListenAnyIP(5000))
                                     .UseContentRoot(Directory.GetCurrentDirectory())
                                     .UseIIS()
                                     .UseSerilog((context, config) =>
                                     {
                                         var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Logs/isotope-.txt");
                                         config
                                             .Enrich.FromLogContext()
                                             .MinimumLevel.Information()
                                             .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                                             .WriteTo.Console()
                                             .WriteTo.Debug()
                                             .WriteTo.File(path, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7);
                                     })
                                     .UseStartup<Startup>();
                       })
                       .ConfigureServices(services =>
                       {
                           services.AddSingleton<BackgroundJobService>();
                           services.AddSingleton<IBackgroundJobService>(sp => sp.GetService<BackgroundJobService>());
                           services.AddHostedService(sp => sp.GetService<BackgroundJobService>());

                           services.AddTransient<RebuildInheritedTagsJob>();
                       });
        }
    }
}
