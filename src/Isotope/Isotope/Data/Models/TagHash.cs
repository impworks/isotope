using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isotope.Data.Models;

/// <summary>
/// Unique person hash related to a tag.
/// </summary>
public class TagHash
{
    /// <summary>
    /// Surrogate ID.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Hash value.
    /// </summary>
    [StringLength(500)]
    public string Value { get; set; }

    /// <summary>
    /// Related tag.
    /// </summary>
    public Tag Tag { get; set; }
}