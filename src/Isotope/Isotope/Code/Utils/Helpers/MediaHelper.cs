using System;
using System.IO;

namespace Isotope.Code.Utils.Helpers
{
    /// <summary>
    /// Helper utilities for working with media files.
    /// </summary>
    public static class MediaHelper
    {
        /// <summary>
        /// Gets the file path for a media frame of specified size.
        /// </summary>
        public static string GetSizedMediaPath(string fullPath, MediaSize size)
        {
            if (string.IsNullOrEmpty(fullPath))
                return fullPath;

            if (size == MediaSize.Original)
                return fullPath;

            if (size == MediaSize.Large)
                return Path.ChangeExtension(fullPath, ".lg.jpg");

            if (size == MediaSize.Small)
                return Path.ChangeExtension(fullPath, ".sm.jpg");

            throw new ArgumentOutOfRangeException(nameof(size), "Unexpected media size!");
        }
    }
}