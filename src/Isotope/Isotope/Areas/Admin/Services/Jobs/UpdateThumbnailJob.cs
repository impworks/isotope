using System;
using System.Threading;
using System.Threading.Tasks;
using Isotope.Code.Services.Jobs;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using SixLabors.ImageSharp;

namespace Isotope.Areas.Admin.Services.Jobs;

/// <summary>
/// Background job for updating the thumbnail file after the thumbnail area has been updated in UI.
/// </summary>
public class UpdateThumbnailJob(AppDbContext db) : JobBase<string>
{
    protected override async Task ExecuteAsync(string key, CancellationToken token)
    {
        var media = await db.Media.GetAsync(x => x.Key == key, $"Failed to regenerate thumbnail for media '{key}': it does not exist.");
        var path = MediaHelper.GetFullMediaPath(media.Path);
        var largePath = MediaHelper.GetSizedMediaPath(path, MediaSize.Large);
        var thumbPath = MediaHelper.GetSizedMediaPath(path, MediaSize.Small);

        var preset = ImageHelper.ImagePresets[MediaSize.Small];
        using var src = await Image.LoadAsync(largePath, token);
        using var dst = ImageHelper.GetPortion(src, media.ThumbnailRect, preset.Size);

        await dst.SaveAsync(thumbPath, preset.Codec, token);

        media.VersionDate = DateTime.Now;
        await db.SaveChangesAsync(CancellationToken.None);
    }
}