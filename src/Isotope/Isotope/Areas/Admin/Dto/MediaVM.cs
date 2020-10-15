using Isotope.Code.Utils;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class MediaVM: IMapped
    {
        public string Description { get; set; }
        public string Date { get; set; }
        public TagBindingVM[] ExtraTags { get; set; }
        public OverlayTagBindingVM[] OverlayTags { get; set; }
        
        public void Configure(TypeAdapterConfig config)
        {
            // todo
        }
    }
}