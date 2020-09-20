using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Isotope.Data.Models
{
    /// <summary>
    /// A folder in the hierarchy.
    /// </summary>
    public class Folder
    {
        /// <summary>
        /// Unique random key of the folder.
        /// </summary>
        [Key]
        [StringLength(50)]
        public string Key { get; set; }
        
        /// <summary>
        /// Readable name of the folder.
        /// </summary>
        [StringLength(200)]
        public string Caption { get; set; }

        /// <summary>
        /// Folder's name in the URL.
        /// </summary>
        [StringLength(200)]
        public string Slug { get; set; }

        /// <summary>
        /// Full relative URL to this folder, including all parent folder names.
        /// </summary>
        [StringLength(500)]
        public string Path { get; set; }

        /// <summary>
        /// Nesting level.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Cached number of photos within this folder.
        /// </summary>
        public int MediaCount { get; set; }

        /// <summary>
        /// Media to display as the folder's preview.
        /// </summary>
        public Media Thumbnail { get; set; }
        
        /// <summary>
        /// FK for thumbnail.
        /// </summary>
        public string ThumbnailKey { get; set; }

        /// <summary>
        /// Tags attributed to this folder.
        /// </summary>
        public ICollection<FolderTagBinding> Tags { get; set; }
    }
}
