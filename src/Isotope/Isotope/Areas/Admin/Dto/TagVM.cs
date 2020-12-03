using System.Linq;
using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class TagVM: IMapped
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public TagType Type { get; set; }
        public int Count { get; set; }
        
        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Tag, TagVM>()
                  .Map(x => x.Id, x => x.Id)
                  .Map(x => x.Caption, x => x.Caption)
                  .Map(x => x.Type, x => x.Type)
                  .Map(x => x.Count, x => x.MediaTagBindings.Count());

            config.NewConfig<TagVM, Tag>()
                  .Map(x => x.Caption, x => x.Caption)
                  .Map(x => x.Type, x => x.Type)
                  .Ignore(x => x.Id);
        }
    }
}