using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Isotope.Code.Services.Jobs;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace Isotope.Areas.Admin.Services.Jobs;

/// <summary>
/// Background job for extracting EXIF metadata from existing media files.
/// Processes files that don't have metadata yet (CameraModel is null).
/// </summary>
public class ExtractMediaMetadataJob(AppDbContext db, ILogger logger) : JobBase
{
    private const int BatchSize = 50;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        var totalProcessed = 0;
        var totalUpdated = 0;

        while (!token.IsCancellationRequested)
        {
            // Get a batch of media without EXIF data (only JPEGs have EXIF)
            var batch = await db.Media
                .Where(x => x.CameraModel == null && x.Type == MediaType.Photo)
                .OrderBy(x => x.UploadDate)
                .Take(BatchSize)
                .ToListAsync(token);

            if (batch.Count == 0)
                break;

            foreach (var media in batch)
            {
                if (token.IsCancellationRequested)
                    break;

                try
                {
                    var updated = await ExtractMetadataAsync(media, token);
                    if (updated)
                        totalUpdated++;
                }
                catch (Exception ex)
                {
                    logger.Warning(ex.Demystify(), "Failed to extract metadata for media '{Key}'", media.Key);
                    // Mark as processed (empty string) to avoid reprocessing
                    media.CameraModel = "";
                }

                totalProcessed++;
            }

            await db.SaveChangesAsync(CancellationToken.None);

            logger.Information(
                "ExtractMediaMetadataJob: Processed {Processed} files, {Updated} with metadata",
                totalProcessed, totalUpdated);
        }

        logger.Information(
            "ExtractMediaMetadataJob completed: {Processed} files processed, {Updated} with metadata extracted",
            totalProcessed, totalUpdated);
    }

    /// <summary>
    /// Extracts EXIF metadata from a single media file.
    /// </summary>
    private async Task<bool> ExtractMetadataAsync(Media media, CancellationToken token)
    {
        var path = MediaHelper.GetFullMediaPath(media.Path);

        using var image = await Image.LoadAsync(path, token);
        var exif = image.Metadata.ExifProfile;

        if (exif == null)
        {
            // No EXIF data, mark as processed
            media.CameraModel = "";
            return false;
        }

        // Camera info
        media.CameraMake = GetExifString(exif, ExifTag.Make)?.Trim();
        media.CameraModel = GetExifString(exif, ExifTag.Model)?.Trim() ?? "";
        media.LensModel = GetExifString(exif, ExifTag.LensModel)?.Trim();

        // Exposure settings
        media.ExposureTime = FormatExposureTime(GetExifRational(exif, ExifTag.ExposureTime));
        media.FNumber = FormatFNumber(GetExifRational(exif, ExifTag.FNumber));
        media.IsoSpeed = GetExifUShortArray(exif, ExifTag.ISOSpeedRatings)?.FirstOrDefault();
        media.FocalLength = FormatFocalLength(GetExifRational(exif, ExifTag.FocalLength));

        // GPS coordinates
        var (lat, lng) = ParseGpsCoordinates(exif);
        media.Latitude = lat;
        media.Longitude = lng;

        // Also update date if not set
        if (string.IsNullOrEmpty(media.Date))
        {
            var dateStr = GetExifString(exif, ExifTag.DateTimeOriginal);
            if (!string.IsNullOrEmpty(dateStr))
            {
                var date = ParseDate(dateStr);
                if (date.HasValue)
                    media.Date = date.Value.ToString("yyyy.MM.dd");
            }
        }

        return !string.IsNullOrEmpty(media.CameraMake) ||
               !string.IsNullOrEmpty(media.LensModel) ||
               media.IsoSpeed.HasValue ||
               media.Latitude.HasValue;
    }

    #region EXIF Helpers (same as JpegMediaHandler)

    private static string GetExifString(ExifProfile profile, ExifTag<string> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    private static Rational? GetExifRational(ExifProfile profile, ExifTag<Rational> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    private static ushort[] GetExifUShortArray(ExifProfile profile, ExifTag<ushort[]> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    private static Rational[] GetExifRationalArray(ExifProfile profile, ExifTag<Rational[]> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    private static string FormatExposureTime(Rational? rational)
    {
        if (rational == null)
            return null;

        var value = rational.Value.ToDouble();
        if (value <= 0)
            return null;

        if (value >= 1)
            return $"{value:0.#}s";

        var denominator = (int)Math.Round(1.0 / value);
        return $"1/{denominator}";
    }

    private static string FormatFNumber(Rational? rational)
    {
        if (rational == null)
            return null;

        var value = rational.Value.ToDouble();
        if (value <= 0)
            return null;

        return value % 1 == 0 ? $"f/{value:0}" : $"f/{value:0.#}";
    }

    private static string FormatFocalLength(Rational? rational)
    {
        if (rational == null)
            return null;

        var value = rational.Value.ToDouble();
        if (value <= 0)
            return null;

        return value % 1 == 0 ? $"{value:0}mm" : $"{value:0.#}mm";
    }

    private static (double? lat, double? lng) ParseGpsCoordinates(ExifProfile profile)
    {
        var latValues = GetExifRationalArray(profile, ExifTag.GPSLatitude);
        var lngValues = GetExifRationalArray(profile, ExifTag.GPSLongitude);

        if (latValues == null || lngValues == null || latValues.Length < 3 || lngValues.Length < 3)
            return (null, null);

        profile.TryGetValue(ExifTag.GPSLatitudeRef, out var latRef);
        profile.TryGetValue(ExifTag.GPSLongitudeRef, out var lngRef);

        var lat = ConvertGpsToDecimal(latValues);
        var lng = ConvertGpsToDecimal(lngValues);

        if (latRef?.Value == "S")
            lat = -lat;
        if (lngRef?.Value == "W")
            lng = -lng;

        return (lat, lng);
    }

    private static double ConvertGpsToDecimal(Rational[] values)
    {
        var degrees = values[0].ToDouble();
        var minutes = values[1].ToDouble();
        var seconds = values[2].ToDouble();

        return degrees + (minutes / 60.0) + (seconds / 3600.0);
    }

    private static DateTime? ParseDate(string dateRaw)
    {
        if (string.IsNullOrEmpty(dateRaw))
            return null;

        var dateFixed = Regex.Replace(dateRaw, @"^(?<year>\d{4}):(?<month>\d{2}):(?<day>\d{2})",
            "${year}/${month}/${day}");

        if (DateTime.TryParse(dateFixed, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        return null;
    }

    #endregion
}
