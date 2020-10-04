using System;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    /// <summary>
    /// Details of a shared link.
    /// </summary>
    public class SharedLinkDetailsVM: SharedLinkVM
    {
        public string Key { get; set; }
        public string FolderCaption { get; set; }
        public string TagCount { get; set; }

        public override void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<SharedLink, SharedLinkDetailsVM>()
                  .Map(x => x.Mode, x => x.Mode)
                  .Map(x => x.Tags, x => x.Tags)
                  .Map(x => x.DateFrom, x => x.DateFrom)
                  .Map(x => x.DateTo, x => x.DateTo)
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Folder, x => x.Folder.Path)
                  .Map(x => x.FolderCaption, x => x.Folder.Caption)
                  .Map(x => x.TagCount, x => x.Tags.Split(',', StringSplitOptions.None).Length);
        }
    }
}