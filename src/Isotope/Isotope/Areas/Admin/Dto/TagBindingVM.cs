using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto;

public class TagBindingVM: IMapped
{
    public int TagId { get; set; }
    public TagBindingType Type { get; set; }
        
    public virtual void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<MediaTagBinding, TagBindingVM>()
              .Map(x => x.TagId, x => x.TagId)
              .Map(x => x.Type, x => x.Type);
            
        config.NewConfig<TagBindingVM, MediaTagBinding>()
              .Map(x => x.TagId, x => x.TagId)
              .Map(x => x.Type, x => x.Type)
              .IgnoreNonMapped(true);
    }
}