using System;
using System.Collections.Generic;
using System.Drawing;
using Isotope.Data.Models;

namespace Isotope.Areas.Admin.Services.MediaHandlers
{
    /// <summary>
    /// Details of a media processed by a handler.
    /// </summary>
    public class MediaInfo
    {
        /// <summary>
        /// Original image extracted for thumbnail generation.
        /// </summary>
        public Image FullImage { get; set; }
        
        /// <summary>
        /// Type of the media.
        /// </summary>
        public MediaType MediaType { get; set; }
        
        /// <summary>
        /// Flag indicating that no further post-processing is required for the media.
        /// </summary>
        public bool IsReady { get; set; }
        
        /// <summary>
        /// Media creation date (from metadata).
        /// </summary>
        public DateTime? Date { get; set; }
        
        /// <summary>
        /// Additional metadata (EXIF, etc).
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }
    }
}