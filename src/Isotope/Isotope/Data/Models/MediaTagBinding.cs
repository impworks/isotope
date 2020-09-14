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

    /// <summary>
    /// Location of the tagged element on the photo.
    /// </summary>
    public class MediaTagBindingLocation
    {
        /// <summary>
        /// X coordinate of the left top corner (in 0..1 coordinate space).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate of the left top corner (in 0..1 coordinate space).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Width of the bounding box (in 0..1 coordinate space).
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height of the bounding box (in 0..1 coordinate space).
        /// </summary>
        public double Height { get; set; }
    }
}
