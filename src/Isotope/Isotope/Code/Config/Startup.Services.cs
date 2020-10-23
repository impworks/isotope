using Isotope.Areas.Admin.Services;
using Isotope.Areas.Admin.Services.MediaHandlers;
using Isotope.Areas.Front.Services;
using Isotope.Code.Services;
using Isotope.Code.Services.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Isotope.Code.Config
{
    public partial class Startup
    {
        /// <summary>
        /// Registers application logic services.
        /// </summary>
        private void ConfigureAppServices(IServiceCollection services)
        {
            // global
            services.AddSingleton<CacheService>();

            // frontend
            services.AddScoped<AuthService>();
            services.AddScoped<ConfigService>();
            services.AddScoped<FolderPresenter>();
            services.AddScoped<TagsPresenter>();
            services.AddScoped<MediaPresenter>();
            services.AddScoped<UserContextManager>();
            services.AddScoped<GalleryInfoPresenter>();
            
            services.AddScoped<IMediaHandler, JpegMediaHandler>();

            // admin
            services.AddScoped<TagManager>();
            services.AddScoped<SharedLinkManager>();
            services.AddScoped<UserManager>();
            services.AddScoped<ConfigManager>();
            services.AddScoped<FolderManager>();
            services.AddScoped<MediaManager>();
        }
    }
}