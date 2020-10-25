using Impworks.Utils.Linq;
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
        public int[] Tags { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public SearchScope Scope { get; set; }
        
        public virtual void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<SharedLinkVM, SharedLink>()
                  .Map(x => x.Scope, x => x.Scope)
                  .Map(x => x.Tags, x => x.Tags == null ? null : x.Tags.JoinString(","))
                  .Map(x => x.DateFrom, x => x.From)
                  .Map(x => x.DateTo, x => x.To)
                  .Ignore(x => x.Folder)
                  .Ignore(x => x.Key);
        }
    }
}