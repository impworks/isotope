using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Isotope.Code.Utils.Date;
using Isotope.Code.Utils.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Isotope.Code.Config;

public partial class Startup
{
    /// <summary>
    /// Configures and registers MVC-related services.
    /// </summary>
    private void ConfigureMvcServices(IServiceCollection services)
    {
        services.AddSingleton(_ => Configuration);
        services.AddSingleton(_ => Log.Logger);
        services.AddScoped<JwtAuthorizeFilter>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        services.ConfigureApplicationCookie(x =>
        {
            x.LoginPath = new PathString("/");
            x.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = ctx =>
                {
                    ctx.Response.Clear();
                    ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                }
            };
        });

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

        if (Environment.IsDevelopment())
        {
            services.AddCors(opts => opts.AddDefaultPolicy(x =>
            {
                x.AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowAnyOrigin();
            }));
        }
    }
}