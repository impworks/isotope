using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Impworks.Utils.Linq;
using Isotope.Data.Models;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

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
            var image = await Image.LoadAsync(path);
            image.Mutate(x => x.AutoOrient());

            var media = new MediaInfo
            {
                FullImage = image,
                IsReady = true,
                MediaType = MediaType.Photo
            };

            PopulateMetadata(media);

            return media;
        }
        
        /// <summary>
        /// Extracts additional data from the media.
        /// </summary>
        private void PopulateMetadata(MediaInfo media)
        {
            try
            {
                var exif = media.FullImage.Metadata.ExifProfile;
                if (exif == null)
                    return;

                media.Date = ParseDate(exif.Values.FirstOrDefault(x => x.Tag == ExifTag.DateTimeOriginal)?.GetValue()?.ToString());
                media.ExtraData = GetMetadata(exif);
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
        private Dictionary<string, string> GetMetadata(ExifProfile profile)
        {
            // todo: only extract a couple of "interesting" tags
            var result = new Dictionary<string, string>();
            var entries = profile.Values
                                 .Select(x => new {Tag = x.Tag.ToString(), Value = GetReadableValue(x)})
                                 .Where(x => x.Value?.Length > 0 && x.Value != "0");

            foreach (var entry in entries)
                if (!result.ContainsKey(entry.Tag))
                    result[entry.Tag] = entry.Value;
            
            return result;
            
            string GetReadableValue(IExifValue entry)
            {
                var value = entry.GetValue();
                if (!entry.IsArray)
                    return value?.ToString();

                if(value is IList<byte> bytes && bytes.Any(x => x != 0))
                    return "[" + bytes.JoinString(", ") + "]";

                if (value is IList<short> shorts && shorts.Any(x => x != 0))
                    return "[" + shorts.JoinString(", ") + "]";

                return null;
            }
        }

        #endregion
    }
}