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
            // frontend
            services.AddScoped<AuthService>();
            services.AddScoped<ConfigService>();
            services.AddScoped<FolderPresenter>();
            services.AddScoped<TagsPresenter>();
            services.AddScoped<MediaPresenter>();
            services.AddScoped<UserContextManager>();

            // admin
        }
    }
}