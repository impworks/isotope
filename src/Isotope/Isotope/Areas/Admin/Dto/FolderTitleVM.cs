using System.Collections.Generic;
using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    /// <summary>
    /// Brief information about a folder.
    /// </summary>
    public class FolderTitleVM: IMapped
    {
        public string Key { get; set; }
        public string Caption { get; set; }
        public int MediaCount { get; set; }
        
        public IReadOnlyList<FolderTitleVM> Subfolders { get; set; }

        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<Folder, FolderTitleVM>()
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Caption, x => x.Caption)
                  .Map(x => x.MediaCount, x => x.MediaCount)
                  .Ignore(x => x.Subfolders);
        }
    }
}