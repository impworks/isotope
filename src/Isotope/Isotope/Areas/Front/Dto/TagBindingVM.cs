using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details of a tag related to a folder.
    /// </summary>
    public class TagBindingVM: IMapped
    {
        /// <summary>
        /// Related tag.
        /// </summary>
        public TagVM Tag { get; set; }
        
        /// <summary>
        /// Location of the tagged entity.
        /// </summary>
        public TagBindingLocationVM Location { get; set; }
        
        /// <summary>
        /// Type of the binding.
        /// </summary>
        public TagBindingType Type { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<MediaTagBinding, TagBindingVM>()
                  .Map(x => x.Tag, x => x.Tag)
                  .Map(x => x.Location, x => x.Location)
                  .Map(x => x.Type, x => x.Type);

            config.NewConfig<FolderTagBinding, TagBindingVM>()
                  .Map(x => x.Tag, x => x.Tag)
                  .Map(x => x.Type, x => TagBindingType.Custom)
                  .Ignore(x => x.Location);
        }
    }
}