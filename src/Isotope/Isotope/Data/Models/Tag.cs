using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isotope.Data.Models
{
    /// <summary>
    /// A description that can link related entities across media and folders.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Surrogate ID.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Readable name of the tag.
        /// </summary>
        [StringLength(200)]
        public string Caption { get; set; }

        /// <summary>
        /// Type of the tag.
        /// </summary>
        public TagType Type { get; set; }

        /// <summary>
        /// Related hashes.
        /// </summary>
        public ICollection<TagHash> Hashes { get; set; }
    }
}
