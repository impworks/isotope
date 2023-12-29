using System.ComponentModel.DataAnnotations;

namespace Isotope.Code.Services.Config;

/// <summary>
/// General application configuration.
/// </summary>
public class DynamicConfig
{
    /// <summary>
    /// The title of the website. Displayed in the top bar and browser title.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    /// <summary>
    /// Flag indicating that the website allows unauthorized visitors to view the contents.
    /// </summary>
    public bool AllowGuests { get; set; }
}