using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Isotope.Code.Utils.Helpers;
using Isotope.Data.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Serilog;

namespace Isotope.Areas.Admin.Services.MediaHandlers
{
    /// <summary>
    /// Media handler to process JPEG images.
    /// </summary>
    public class JpegMediaHandler: IMediaHandler
    {
        public JpegMediaHandler(ILogger logger)
        {
            _logger = logger;
        }
        
        private readonly ILogger _logger;
        
        /// <summary>
        /// Known media types.
        /// </summary>
        public string[] SupportedMimeTypes { get; } = {"image/jpeg"};

        /// <summary>
        /// Media processing.
        /// </summary>
        public async Task<MediaInfo> ProcessAsync(string key, string path)
        {
            var image = await Task.Run(() =>
            {
                var img = Image.FromFile(path);
                ImageHelper.ApplyExifRotation(img);
                return img;
            });

            var media = new MediaInfo
            {
                FullImage = image,
                IsReady = true,
                MediaType = MediaType.Photo
            };

            await PopulateMetadataAsync(media, path);

            return media;
        }
        
        /// <summary>
        /// Extracts additional data from the media.
        /// </summary>
        private async Task PopulateMetadataAsync(MediaInfo media, string path)
        {
            try
            {
                var dirs = await Task.Run(() => ImageMetadataReader.ReadMetadata(path));
                var exifDir = dirs.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                if (exifDir == null)
                    return;

                media.Date = ParseDate(exifDir.GetDescription(ExifDirectoryBase.TagDateTimeOriginal));
                media.ExtraData = GetMetadata(exifDir);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Demystify(), "Failed to get photo metadata!");
            }
        }

        #region Private helpers

        /// <summary>
        /// Parses the date from an EXIF raw value.
        /// </summary>
        private DateTime? ParseDate(string dateRaw)
        {
            if (string.IsNullOrEmpty(dateRaw))
                return null;

            var dateFixed = Regex.Replace(dateRaw, @"^(?<year>\d{4}):(?<month>\d{2}):(?<day>\d{2})", "${year}/${month}/${day}");

            if (DateTime.TryParse(dateFixed, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            return null;
        }

        /// <summary>
        /// Returns metadata entries.
        /// </summary>
        private Dictionary<string, string> GetMetadata(ExifSubIfdDirectory dir)
        {
            // todo: only extract a couple of "interesting" tags
            return dir.Tags.ToDictionary(x => x.Name, x => x.Description);
        }

        #endregion
    }
}