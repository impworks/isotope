using System.Collections.Generic;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto;

/// <summary>
/// Brief information about a folder.
/// </summary>
public class FolderTitleVM: IMapped
{
    public string Key { get; set; }
    public int Depth { get; set; }
    public string Caption { get; set; }
    public string Slug { get; set; }
    public int MediaCount { get; set; }
    public string Path { get; set; }
    public string ThumbnailPath { get; set; }
        
    public IReadOnlyList<FolderTitleVM> Subfolders { get; set; }

    public void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<Folder, FolderTitleVM>()
              .Map(x => x.Key, x => x.Key)
              .Map(x => x.Depth, x => x.Depth)
              .Map(x => x.Caption, x => x.Caption)
              .Map(x => x.Slug, x => x.Slug)
              .Map(x => x.MediaCount, x => x.MediaCount)
              .Map(x => x.Path, x => x.Path)
              .Map(x => x.ThumbnailPath, x => x.Thumbnail == null ? null : MediaHelper.GetThumbnailUrl(x.Thumbnail))
              .Ignore(x => x.Subfolders);
    }
}