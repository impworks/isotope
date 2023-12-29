using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto;

/// <summary>
/// Details of a tag (for either folder or a media).
/// </summary>
public class TagVM: IMapped
{
    /// <summary>
    /// Unique ID to use for filtering by this tag.
    /// </summary>
    public int Id { get; set; }
        
    /// <summary>
    /// Readable caption.
    /// </summary>
    public string Caption { get; set; }
        
    /// <summary>
    /// Type of the tag.
    /// </summary>
    public TagType Type { get; set; }

    public void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<Tag, TagVM>()
              .Map(x => x.Id, x => x.Id)
              .Map(x => x.Caption, x => x.Caption)
              .Map(x => x.Type, x => x.Type);
    }
}