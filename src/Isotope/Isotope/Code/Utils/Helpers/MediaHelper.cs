using System;
using System.Collections.Generic;
using System.IO;
using Isotope.Data.Models;

namespace Isotope.Code.Utils.Helpers;

/// <summary>
/// Helper utilities for working with media files.
/// </summary>
public static class MediaHelper
{
    /// <summary>
    /// Gets the file path for a media frame of specified size.
    /// </summary>
    public static string GetSizedMediaPath(string path, MediaSize size)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        if (size == MediaSize.Original)
            return path;

        if (size == MediaSize.Large)
            return Path.ChangeExtension(path, ".lg.jpg");

        if (size == MediaSize.Small)
            return Path.ChangeExtension(path, ".sm.jpg");

        throw new ArgumentOutOfRangeException(nameof(size), "Unexpected media size!");
    }

    /// <summary>
    /// Returns the full URL for media thumbnail with nonce.
    /// </summary>
    public static string GetThumbnailUrl(Media media)
    {
        if (media == null)
            return null;
        
        var sizedPath = GetSizedMediaPath(media.Path, MediaSize.Small);
        var nonce = new DateTimeOffset(media.VersionDate.ToUniversalTime(), TimeSpan.Zero).ToUnixTimeSeconds();
        return sizedPath + "?nonce=" + nonce;
    }

    /// <summary>
    /// Returns the full path by relative path to data root (as stored in DB).
    /// </summary>
    public static string GetFullMediaPath(string path)
    {
        var parts = new List<string> {Directory.GetCurrentDirectory(), "App_Data"};
        parts.AddRange(path.Split('/', StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(parts.ToArray());
    }
}