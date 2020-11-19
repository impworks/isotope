using System;
using System.Linq;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class MediaThumbnailVM: IMapped
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
        
        /// <summary>
        /// Date of the media's upload.
        /// </summary>
        public DateTime UploadDate { get; set; }
        
        /// <summary>
        /// Number of tags on the media.
        /// </summary>
        public int Tags { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Media, MediaThumbnailVM>()
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Type, x => x.Type)
                  .Map(x => x.UploadDate, x => x.UploadDate)
                  .Map(x => x.Tags, x => x.Tags.Count(y => y.Type != TagBindingType.Inherited))
                  .Map(x => x.ThumbnailPath, x => MediaHelper.GetThumbnailUrl(x));
        }
    }
}