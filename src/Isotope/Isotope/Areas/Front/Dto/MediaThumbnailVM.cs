using Isotope.Data.Models;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Brief information about a media file in a folder.
    /// </summary>
    public class MediaThumbnailVM
    {
        /// <summary>
        /// Unique key of the media file.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Type of the media (Photo, Video).
        /// </summary>
        public MediaType Type { get; set; }
        
        /// <summary>
        /// Path to the thumbnail for this media. 
        /// </summary>
        public string ThumbnailPath { get; set; }
    }
}