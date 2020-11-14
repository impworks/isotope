using System.Linq;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    /// <summary>
    /// Media details.
    /// </summary>
    public class MediaVM: IMapped
    {
        public string FullPath { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public TagBindingVM[] ExtraTags { get; set; }
        public OverlayTagBindingVM[] OverlayTags { get; set; }
        
        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Media, MediaVM>()
                  .Map(x => x.FullPath, x => MediaHelper.GetSizedMediaPath(x.Path, MediaSize.Large))
                  .Map(x => x.Description, x => x.Description)
                  .Map(x => x.Date, x => x.Date)
                  .Map(x => x.OverlayTags, x => x.Tags.Where(y => y.Type == TagBindingType.Depicted))
                  .Map(x => x.ExtraTags, x => x.Tags.Where(y => y.Type == TagBindingType.Author || y.Type == TagBindingType.Custom));

            config.NewConfig<MediaVM, Media>()
                  .Map(x => x.Description, x => x.Description)
                  .Map(x => x.Date, x => x.Date)
                  .IgnoreNonMapped(true);
        }
    }
}