using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class OverlayTagBindingVM: TagBindingVM
    {
        public RectVM Location { get; set; }

        public override void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<MediaTagBinding, OverlayTagBindingVM>()
                  .Inherits<MediaTagBinding, TagBindingVM>()
                  .Map(x => x.Location, x => x.Location);
            
            config.NewConfig<OverlayTagBindingVM, MediaTagBinding>()
                  .Inherits<TagBindingVM, MediaTagBinding>()
                  .Map(x => x.Location, x => x.Location);
        }
    }
}