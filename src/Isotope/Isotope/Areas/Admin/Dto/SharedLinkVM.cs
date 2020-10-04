using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    /// <summary>
    /// Details for creating a shared link.
    /// </summary>
    public class SharedLinkVM: IMapped
    {
        public string Folder { get; set; }
        public string Tags { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public SearchMode Mode { get; set; }
        
        public virtual void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<SharedLinkVM, SharedLink>()
                  .Map(x => x.Mode, x => x.Mode)
                  .Map(x => x.Tags, x => x.Tags)
                  .Map(x => x.DateFrom, x => x.DateFrom)
                  .Map(x => x.DateTo, x => x.DateTo)
                  .Ignore(x => x.Folder)
                  .Ignore(x => x.Key);
        }
    }
}