using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Brief information about a media file in a folder.
    /// </summary>
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

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Media, MediaThumbnailVM>()
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Type, x => x.Type)
                  .Map(x => x.ThumbnailPath, x => MediaHelper.GetThumbnailUrl(x));
        }
    }
}