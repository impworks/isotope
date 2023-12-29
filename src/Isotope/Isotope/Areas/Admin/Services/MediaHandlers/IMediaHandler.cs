using System.Threading.Tasks;

namespace Isotope.Areas.Admin.Services.MediaHandlers;

/// <summary>
/// Common interface for media handlers.
/// </summary>
public interface IMediaHandler
{
    /// <summary>
    /// Media types supported by this media handler.
    /// </summary>
    string[] SupportedMimeTypes { get; }

    /// <summary>
    /// Parses the saved media, creating a thumbnail image and detecting 
    /// </summary>
    Task<MediaInfo> ProcessAsync(string key, string path);
}