using System.IO;
using Isotope.Code.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Isotope
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.UseKestrel()
                                     .UseContentRoot(Directory.GetCurrentDirectory())
                                     .UseIIS()
                                     .UseStartup<Startup>();
                       });
        }
    }
}
