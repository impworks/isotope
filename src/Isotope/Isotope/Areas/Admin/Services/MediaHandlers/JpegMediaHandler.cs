using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Isotope.Data.Models;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

namespace Isotope.Areas.Admin.Services.MediaHandlers;

/// <summary>
/// Media handler to process JPEG images.
/// </summary>
public class JpegMediaHandler(ILogger logger) : IMediaHandler
{
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

            // Date
            media.Date = ParseDate(GetExifString(exif, ExifTag.DateTimeOriginal));

            // Camera info
            media.CameraMake = GetExifString(exif, ExifTag.Make)?.Trim();
            media.CameraModel = GetExifString(exif, ExifTag.Model)?.Trim();
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
        }
        catch (Exception ex)
        {
            logger.Error(ex.Demystify(), "Failed to get photo metadata!");
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

        var dateFixed = Regex.Replace(dateRaw, @"^(?<year>\d{4}):(?<month>\d{2}):(?<day>\d{2})",
            "${year}/${month}/${day}");

        if (DateTime.TryParse(dateFixed, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        return null;
    }

    /// <summary>
    /// Gets a string value from EXIF profile.
    /// </summary>
    private static string GetExifString(ExifProfile profile, ExifTag<string> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    /// <summary>
    /// Gets a rational value from EXIF profile.
    /// </summary>
    private static Rational? GetExifRational(ExifProfile profile, ExifTag<Rational> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    /// <summary>
    /// Gets a ushort array from EXIF profile.
    /// </summary>
    private static ushort[] GetExifUShortArray(ExifProfile profile, ExifTag<ushort[]> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    /// <summary>
    /// Gets an array of rationals from EXIF profile.
    /// </summary>
    private static Rational[] GetExifRationalArray(ExifProfile profile, ExifTag<Rational[]> tag)
    {
        return profile.TryGetValue(tag, out var value) ? value.Value : null;
    }

    /// <summary>
    /// Formats exposure time as a readable string (e.g., "1/250").
    /// </summary>
    private static string FormatExposureTime(Rational? rational)
    {
        if (rational == null)
            return null;

        var value = rational.Value.ToDouble();
        if (value <= 0)
            return null;

        if (value >= 1)
            return $"{value:0.#}s";

        // Express as fraction 1/x
        var denominator = (int)Math.Round(1.0 / value);
        return $"1/{denominator}";
    }

    /// <summary>
    /// Formats f-number as a readable string (e.g., "f/2.8").
    /// </summary>
    private static string FormatFNumber(Rational? rational)
    {
        if (rational == null)
            return null;

        var value = rational.Value.ToDouble();
        if (value <= 0)
            return null;

        return value % 1 == 0 ? $"f/{value:0}" : $"f/{value:0.#}";
    }

    /// <summary>
    /// Formats focal length as a readable string (e.g., "50mm").
    /// </summary>
    private static string FormatFocalLength(Rational? rational)
    {
        if (rational == null)
            return null;

        var value = rational.Value.ToDouble();
        if (value <= 0)
            return null;

        return value % 1 == 0 ? $"{value:0}mm" : $"{value:0.#}mm";
    }

    /// <summary>
    /// Parses GPS coordinates from EXIF profile.
    /// </summary>
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

        // Apply reference direction
        if (latRef?.Value == "S")
            lat = -lat;
        if (lngRef?.Value == "W")
            lng = -lng;

        return (lat, lng);
    }

    /// <summary>
    /// Converts GPS degrees/minutes/seconds to decimal degrees.
    /// </summary>
    private static double ConvertGpsToDecimal(Rational[] values)
    {
        var degrees = values[0].ToDouble();
        var minutes = values[1].ToDouble();
        var seconds = values[2].ToDouble();

        return degrees + (minutes / 60.0) + (seconds / 3600.0);
    }

    #endregion
}