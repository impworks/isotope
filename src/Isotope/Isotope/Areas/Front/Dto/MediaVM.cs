using Isotope.Data.Models;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details of a media file.
    /// </summary>
    public class MediaVM
    {
        /// <summary>
        /// Unique key of the media file.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Type of the media (Photo, Video, etc).
        /// </summary>
        public MediaType Type { get; set; }
        
        /// <summary>
        /// Path to the displayed media.
        /// </summary>
        public string FullPath { get; set; }
        
        /// <summary>
        /// Path to the original media (for downloads).
        /// </summary>
        public string OriginalPath { get; set; }
        
        /// <summary>
        /// Date of the media (in rendered format).
        /// </summary>
        public string Date { get; set; }
        
        /// <summary>
        /// Optional description of the media.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Related tags.
        /// </summary>
        public TagBindingVM[] Tags { get; set; }
    }
}