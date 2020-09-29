using System.Linq;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Date;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details of a media file.
    /// </summary>
    public class MediaVM: IMapped
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
        /// Tags to be displayed over the image.
        /// </summary>
        public TagBindingVM[] OverlayTags { get; set; }
        
        /// <summary>
        /// All other tags.
        /// </summary>
        public TagBindingVM[] ExtraTags { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Media, MediaVM>()
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Type, x => x.Type)
                  .Map(x => x.FullPath, x => x.Type == MediaType.Video ? x.Path : MediaHelper.GetSizedMediaPath(x.Path, MediaSize.Large))
                  .Map(x => x.OriginalPath, x => x.Path)
                  .Map(x => x.Date, x => TryFormatDate(x.Date))
                  .Map(x => x.Description, x => x.Description)
                  .Map(x => x.OverlayTags, x => x.Tags.Where(y => y.Location != null))
                  .Map(x => x.ExtraTags, x => x.Tags.Where(y => y.Location == null && y.Type != TagBindingType.Inherited));
        }
        
        /// <summary>
        /// Displays the date as a readable string if it is entered.
        /// </summary>
        private static string TryFormatDate(string date)
        {
            return FuzzyDate.TryParse(date)?.ReadableDate;
        }
    }
}