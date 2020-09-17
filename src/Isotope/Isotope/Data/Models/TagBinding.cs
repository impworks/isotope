using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isotope.Data.Models
{
    /// <summary>
    /// Binding of a tag to a media or a folder.
    /// </summary>
    public class TagBinding
    {
        /// <summary>
        /// Surrogate ID.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// Related tag.
        /// </summary>
        public Tag Tag { get; set; }
    }
}
