using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isotope.Data.Models;

/// <summary>
/// General application configuration.
/// </summary>
public class DynamicConfigWrapper
{
    /// <summary>
    /// Surrogate ID.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Serialized configuration.
    /// </summary>
    public string Value { get; set; }
}