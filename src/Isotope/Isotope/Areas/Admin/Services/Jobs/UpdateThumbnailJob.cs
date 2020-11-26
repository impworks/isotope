using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Isotope.Code.Services.Jobs;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services.Jobs
{
    /// <summary>
    /// Background job for updating the thumbnail file after the thumbnail area has been updated in UI.
    /// </summary>
    public class UpdateThumbnailJob: JobBase<string>
    {
        public UpdateThumbnailJob(AppDbContext db)
        {
            _db = db;
        }
        
        private readonly AppDbContext _db;
        
        protected override async Task ExecuteAsync(string key, CancellationToken token)
        {
            var media = await _db.Media.FirstOrDefaultAsync(x => x.Key == key);
            if(media == null)
                throw new Exception($"Failed to regenerate thumbnail for media '{key}': it does not exist.");

            var path = MediaHelper.GetFullMediaPath(media.Path);
            var largePath = MediaHelper.GetSizedMediaPath(path, MediaSize.Large);
            var thumbPath = MediaHelper.GetSizedMediaPath(path, MediaSize.Small);

            var preset = ImageHelper.ImagePresets[MediaSize.Small];
            using var src = Image.FromFile(largePath);
            using var dst = ImageHelper.GetPortion(src, media.ThumbnailRect, preset.Size);
            
            dst.Save(thumbPath, preset.Codec, preset.CodecArgs);
            
            media.VersionDate = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }
}