using Isotope.Areas.Front.Services;
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
            services.AddScoped<FolderPresenter>();
            services.AddScoped<TagsPresenter>();
            services.AddScoped<MediaPresenter>();
            services.AddScoped<UserContextManager>();

            // admin
        }
    }
}