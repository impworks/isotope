﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Isotope.Data.Models;

public class Media
{
    /// <summary>
    /// Unique random key of the image.
    /// </summary>
    [Key]
    [StringLength(50)]
    public string Key { get; set; }

    /// <summary>
    /// Full path to the image.
    /// </summary>
    [StringLength(500)]
    public string Path { get; set; }
        
    /// <summary>
    /// Date of the media's uploading to the system.
    /// </summary>
    public DateTime UploadDate { get; set; }
        
    /// <summary>
    /// Date of the media's last modification.
    /// </summary>
    public DateTime VersionDate { get; set; }

    /// <summary>
    /// Containing folder.
    /// </summary>
    public Folder Folder { get; set; }

    /// <summary>
    /// ID of the folder that contains this image.
    /// </summary>
    public string FolderKey { get; set; }
        
    /// <summary>
    /// Type of the media.
    /// </summary>
    public MediaType Type { get; set; }

    /// <summary>
    /// Detailed description of the file.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Date of the media's creation in FuzzyDate format.
    /// </summary>
    [StringLength(10)]
    public string Date { get; set; }

    /// <summary>
    /// Order of the media in parent folder, starting from 0.
    /// </summary>
    public int Order { get; set; }
        
    /// <summary>
    /// Rectangle of the image portion used for a thumbnail.
    /// </summary>
    public Rect ThumbnailRect { get; set; }
        
    /// <summary>
    /// Flag indicating that the media has been processed completely and is ready for display.
    /// </summary>
    public bool IsReady { get; set; }
        
    /// <summary>
    /// Media width (original size in pixels).
    /// </summary>
    public int? Width { get; set; }
        
    /// <summary>
    /// Media height (original size in pixels).
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// List of entities tagged on this image.
    /// </summary>
    public virtual ICollection<MediaTagBinding> Tags { get; set; }
}