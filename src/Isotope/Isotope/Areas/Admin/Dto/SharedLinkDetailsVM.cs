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
        public int TagCount { get; set; }

        public override void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<SharedLink, SharedLinkDetailsVM>()
                  .Map(x => x.Scope, x => x.Scope)
                  .Map(x => x.Tags, x => x.Tags)
                  .Map(x => x.From, x => x.DateFrom)
                  .Map(x => x.To, x => x.DateTo)
                  .Map(x => x.Key, x => x.Key)
                  .Map(x => x.Folder, x => x.Folder.Path)
                  .Map(x => x.FolderCaption, x => x.Folder.Caption)
                  .Map(x => x.TagCount, x => x.Tags.Split(',', StringSplitOptions.None).Length);
        }
    }
}