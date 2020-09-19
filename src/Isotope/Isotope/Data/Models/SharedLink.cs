using System.ComponentModel.DataAnnotations;
using Isotope.Areas.Front.Dto;

namespace Isotope.Data.Models
{
    /// <summary>
    /// A unique link that allows bypassing authorization.
    /// </summary>
    public class SharedLink
    {
        /// <summary>
        /// Unique random key of the image.
        /// </summary>
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        
        /// <summary>
        /// Root folder of the shared link.
        /// Can be null.
        /// </summary>
        public Folder Folder { get; set; }
        
        /// <summary>
        /// Flag indicating that subfolders are also displayed when viewing this link.
        /// Otherwise, only media immediately inside this folder are available. 
        /// </summary>
        public SearchMode Mode { get; set; }
        
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
    }
}