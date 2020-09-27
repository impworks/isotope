using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isotope.Data.Models
{
    /// <summary>
    /// An auto-generated tag instance pending moderation.
    /// </summary>
    public class TagSuggestion
    {
        /// <summary>
        /// Surrogate ID.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// Location of the recognized face on the photo.
        /// </summary>
        public Rect Location { get; set; }
        
        /// <summary>
        /// Related photo.
        /// </summary>
        public Media Media { get; set; }
        
        /// <summary>
        /// Related tag (if found by hash match).
        /// </summary>
        public Tag Tag { get; set; }
        
        /// <summary>
        /// Unique hash of the found face.
        /// </summary>
        public string Hash { get; set; }
    }
}