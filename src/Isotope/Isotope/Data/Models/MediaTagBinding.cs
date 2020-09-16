namespace Isotope.Data.Models
{
    /// <summary>
    /// Binding of a tag to a media.
    /// </summary>
    public class MediaTagBinding: TagBinding
    {
        /// <summary>
        /// Location of the tagged entity on the media.
        /// </summary>
        public MediaTagBindingLocation Location { get; set; }

        /// <summary>
        /// Related media file.
        /// </summary>
        public Media Media { get; set; }
    }
}
