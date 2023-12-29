using System;
using System.Collections.Generic;
using Isotope.Data.Models;
using SixLabors.ImageSharp;

namespace Isotope.Areas.Admin.Services.MediaHandlers;

/// <summary>
/// Details of a media processed by a handler.
/// </summary>
public class MediaInfo: IDisposable
{
    /// <summary>
    /// Original image extracted for thumbnail generation.
    /// </summary>
    public Image FullImage { get; set; }
        
    /// <summary>
    /// Type of the media.
    /// </summary>
    public MediaType MediaType { get; set; }
        
    /// <summary>
    /// Flag indicating that no further post-processing is required for the media.
    /// </summary>
    public bool IsReady { get; set; }
        
    /// <summary>
    /// Media creation date (from metadata).
    /// </summary>
    public DateTime? Date { get; set; }
        
    /// <summary>
    /// Additional metadata (EXIF, etc).
    /// </summary>
    public Dictionary<string, string> ExtraData { get; set; }

    /// <summary>
    /// Releases the image resources back to the pool.
    /// </summary>
    public void Dispose()
    {
        FullImage?.Dispose();
    }
}