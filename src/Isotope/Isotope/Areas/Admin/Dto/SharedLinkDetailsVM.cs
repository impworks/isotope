using System;
using System.Linq;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto;

/// <summary>
/// Details of a shared link.
/// </summary>
public class SharedLinkDetailsVM: SharedLinkVM
{
    public DateTime CreationDate { get; set; }
    public string Key { get; set; }
    public string FolderCaption { get; set; }

    public override void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<SharedLink, SharedLinkDetailsVM>()
              .Map(x => x.Caption, x => x.Caption)
              .Map(x => x.Scope, x => x.Scope)
              .Map(x => x.Tags, x => x.Tags == null ? Array.Empty<int>() : x.Tags.Split(',', StringSplitOptions.None).Select(int.Parse))
              .Map(x => x.From, x => x.DateFrom)
              .Map(x => x.To, x => x.DateTo)
              .Map(x => x.Key, x => x.Key)
              .Map(x => x.Folder, x => x.Folder.Path)
              .Map(x => x.FolderCaption, x => x.Folder.Caption)
              .Map(x => x.CreationDate, x => x.CreationDate);
    }
}