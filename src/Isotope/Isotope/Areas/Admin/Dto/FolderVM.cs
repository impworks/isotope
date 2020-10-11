using System.Linq;
using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class FolderVM: IMapped
    {
        public string Key { get; set; }
        public string Caption { get; set; }
        public string Slug { get; set; }
        public string ThumbnailKey { get; set; }
        public int[] Tags { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Folder, FolderVM>()
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Caption, x => x.Caption)
                  .Map(x => x.Slug, x => x.Slug)
                  .Map(x => x.ThumbnailKey, x => x.ThumbnailKey)
                  .Map(x => x.Tags, x => x.Tags.Select(y => y.TagId));

            config.NewConfig<FolderVM, Folder>()
                  .Map(x => x.Caption, x => x.Caption)
                  .Map(x => x.Slug, x => x.Slug)
                  .Map(x => x.ThumbnailKey, x => x.ThumbnailKey)
                  .Ignore(x => x.Key)
                  .Ignore(x => x.Tags);
        }
    }
}