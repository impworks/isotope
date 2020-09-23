using System;
using System.Collections.Generic;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Date;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

            var jwtKey = JwtHelper.GetKey();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opts =>
                    {
                        opts.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = JwtHelper.Issuer,
                            ValidAudience = JwtHelper.Audience,
                            IssuerSigningKey = jwtKey,
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            services.AddMvc(x =>
                    {
                        x.Filters.Add(typeof(CommonExceptionFilterAttribute));
                        x.Filters.Add(typeof(TryAuthorizeFilter));
                    })
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

            if (Configuration.WebServer.RequireHttps)
            {
                services.Configure<MvcOptions>(opts =>
                {
                    opts.Filters.Add(new RequireHttpsAttribute());
                });
            }

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
}
