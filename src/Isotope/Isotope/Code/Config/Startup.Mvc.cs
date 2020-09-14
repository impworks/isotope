using System.Collections.Generic;
using Isotope.Code.Utils.Date;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;

namespace Isotope.Code.Config
{
    public partial class Startup
    {
        /// <summary>
        /// Configures and registers MVC-related services.
        /// </summary>
        private void ConfigureMvcServices(IServiceCollection services)
        {
            services.AddSingleton(p => Configuration);
            services.AddSingleton(p => Log.Logger);

            services.AddMvc()
                    .AddNewtonsoftJson(opts =>
                    {
                        var convs = new List<JsonConverter>
                        {
                            new FuzzyDate.FuzzyDateJsonConverter()
                        };

                        convs.ForEach(x => opts.SerializerSettings.Converters.Add(x));

                        JsonConvert.DefaultSettings = () =>
                        {
                            var settings = new JsonSerializerSettings();
                            convs.ForEach(settings.Converters.Add);
                            return settings;
                        };
                    });

            services.AddRouting(opts =>
            {
                opts.AppendTrailingSlash = false;
                opts.LowercaseUrls = false;
            });

            services.AddSession();

            if (Configuration.WebServer.RequireHttps)
            {
                services.Configure<MvcOptions>(opts =>
                {
                    opts.Filters.Add(new RequireHttpsAttribute());
                });
            }
        }
    }
}
