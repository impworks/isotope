using Isotope.Code.Services.Config;
using Isotope.Code.Utils;
using Mapster;

namespace Isotope.Areas.Admin.Dto;

/// <summary>
/// Updatable configuration properties.
/// </summary>
public class ConfigVM: IMapped
{
    /// <summary>
    /// The title of the website. Displayed in the top bar and browser title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Flag indicating that the website allows unauthorized visitors to view the contents.
    /// </summary>
    public bool AllowGuests { get; set; }

    public void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<ConfigVM, DynamicConfig>()
              .Map(x => x.Title, x => x.Title)
              .Map(x => x.AllowGuests, x => x.AllowGuests);
            
        config.NewConfig<DynamicConfig, ConfigVM>()
              .Map(x => x.Title, x => x.Title)
              .Map(x => x.AllowGuests, x => x.AllowGuests);
    }
}