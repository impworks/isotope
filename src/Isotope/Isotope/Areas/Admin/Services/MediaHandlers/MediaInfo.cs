using System;
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

    // ----- EXIF Metadata -----

    /// <summary>
    /// Camera manufacturer.
    /// </summary>
    public string CameraMake { get; set; }

    /// <summary>
    /// Camera model.
    /// </summary>
    public string CameraModel { get; set; }

    /// <summary>
    /// Lens model.
    /// </summary>
    public string LensModel { get; set; }

    /// <summary>
    /// Exposure time as formatted string (e.g., "1/250").
    /// </summary>
    public string ExposureTime { get; set; }

    /// <summary>
    /// F-number/aperture as formatted string (e.g., "f/2.8").
    /// </summary>
    public string FNumber { get; set; }

    /// <summary>
    /// ISO speed rating.
    /// </summary>
    public int? IsoSpeed { get; set; }

    /// <summary>
    /// Focal length as formatted string (e.g., "50mm").
    /// </summary>
    public string FocalLength { get; set; }

    /// <summary>
    /// GPS latitude coordinate.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// GPS longitude coordinate.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Releases the image resources back to the pool.
    /// </summary>
    public void Dispose()
    {
        FullImage?.Dispose();
    }
}