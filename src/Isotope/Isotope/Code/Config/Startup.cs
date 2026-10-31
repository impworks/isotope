using System;
using System.Globalization;
using System.IO;
using Isotope.Code.Services.Config;
using Isotope.Code.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Isotope.Code.Config;

public partial class Startup
{
    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
                      .SetBasePath(env.ContentRootPath)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                      .AddEnvironmentVariables();

        Configuration = builder.Build().Get<StaticConfig>();
        Environment = env;
    }

    private StaticConfig Configuration { get; }
    private IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Registers all required services in the dependency injection container.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDatabaseServices(services);
        ConfigureMvcServices(services);
        ConfigureMapsterServices(services);
        ConfigureAppServices(services);
    }

    /// <summary>
    /// Configures the web framework pipeline.
    /// </summary>
    public void Configure(IApplicationBuilder app)
    {
        if (Environment.IsDevelopment())
        {
            app.UseSerilogRequestLogging();
            app.UseCors();

            if (Configuration.Debug.Latency > 0)
                app.UseMiddleware<SimulatedLatencyMiddleware>(Configuration.Debug.Latency.Value);
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (Configuration.WebServer.RequireHttps)
            app.UseHttpsRedirection();

        if (Configuration.Debug.DetailedExceptions)
            app.UseDeveloperExceptionPage();

        InitDatabase(app);

        app.UseForwardedHeaders(GetForwardedHeadersOptions())
           .UseStaticFiles()
           .UseStaticFiles(GetAppDataStaticFilesProvider())
           .UseRouting()
           .UseAuthentication()
           .UseAuthorization()
           .UseRequestLocalization(GetRequestLocalizationOptions())
           .UseCookiePolicy()
           .UseEndpoints(x => { x.MapControllers(); });
    }

    /// <summary>
    /// Configures the options for header forwarding.
    /// </summary>
    private ForwardedHeadersOptions GetForwardedHeadersOptions()
    {
        var opts = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
        };
        opts.KnownProxies.Clear();
        opts.KnownNetworks.Clear();
        return opts;
    }

    /// <summary>
    /// Configures the options for request localization.
    /// </summary>
    private RequestLocalizationOptions GetRequestLocalizationOptions()
    {
        var culture = CultureInfo.GetCultureInfo("ru-RU");
        return new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture),
            SupportedCultures = new[] {culture},
            SupportedUICultures = new[] {culture}
        };
    }

    /// <summary>
    /// Configures the static file provider to retrieve files from App_Data/@media.
    /// </summary>
    private StaticFileOptions GetAppDataStaticFilesProvider()
    {
        return new StaticFileOptions
        {
            RequestPath = new PathString("/@media"),
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "@media")
            ),
            OnPrepareResponse = ctx =>
            {
                // File names are unique keys and never reused, so the content is immutable.
                // The only exception is the small thumbnail, which is regenerated in place when the crop changes:
                // its URLs carry a version nonce, so it's only safe to cache forever when the nonce is present.
                var req = ctx.Context.Request;
                var isMutable = req.Path.Value?.EndsWith(".sm.jpg", StringComparison.OrdinalIgnoreCase) == true
                                && !req.Query.ContainsKey("nonce");

                ctx.Context.Response.Headers.CacheControl = isMutable
                    ? "private, no-cache"
                    : "private, max-age=31536000, immutable";
            }
        };
    }
}