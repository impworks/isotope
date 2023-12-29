using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Isotope.Code.Config;

public partial class Startup
{
    /// <summary>
    /// Builds all Mapster mappings and registers the mapper as a service.
    /// </summary>
    private void ConfigureMapsterServices(IServiceCollection services)
    {
        var config = new TypeAdapterConfig
        {
            RequireExplicitMapping = true,
        };
            
        foreach(var map in RuntimeHelper.GetAllInstances<IMapped>())
            map.Configure(config);
            
        config.Compile();
        var mapper = new Mapper(config);
            
        services.AddSingleton<IMapper>(mapper);
    }
}