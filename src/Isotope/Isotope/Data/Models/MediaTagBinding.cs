namespace Isotope.Data.Models;

/// <summary>
/// Binding of a tag to a media.
/// </summary>
public class MediaTagBinding: TagBinding
{
    /// <summary>
    /// Location of the tagged entity on the media.
    /// </summary>
    public Rect Location { get; set; }

    /// <summary>
    /// Related media file.
    /// </summary>
    public Media Media { get; set; }
        
    /// <summary>
    /// FK of the media.
    /// </summary>
    public string MediaKey { get; set; }
        
    /// <summary>
    /// Type of the binding.
    /// </summary>
    public TagBindingType Type { get; set; }
}