using System;
using System.ComponentModel.DataAnnotations;
using Isotope.Areas.Front.Dto;

namespace Isotope.Data.Models;

/// <summary>
/// A unique link that allows bypassing authorization.
/// </summary>
public class SharedLink
{
    /// <summary>
    /// Unique random key of the link.
    /// </summary>
    [Key]
    [StringLength(50)]
    public string Key { get; set; }
        
    /// <summary>
    /// Readable name of the link (for management purposes).
    /// </summary>
    [StringLength(200)]
    public string Caption { get; set; }
        
    /// <summary>
    /// Root folder of the shared link.
    /// Can be null.
    /// </summary>
    public Folder Folder { get; set; }
        
    /// <summary>
    /// Flag indicating that subfolders are also displayed when viewing this link.
    /// Otherwise, only media immediately inside this folder are available. 
    /// </summary>
    public SearchScope Scope { get; set; }
        
    /// <summary>
    /// List of tag IDs, comma-separated.
    /// </summary>
    public string Tags { get; set; }
        
    /// <summary>
    /// Earliest available date.
    /// </summary>
    public string DateFrom { get; set; }
        
    /// <summary>
    /// Latest available date.
    /// </summary>
    public string DateTo { get; set; }
        
    /// <summary>
    /// Date of the link's creation.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Number of times the link has been visited.
    /// </summary>
    public int VisitCount { get; set; }
}