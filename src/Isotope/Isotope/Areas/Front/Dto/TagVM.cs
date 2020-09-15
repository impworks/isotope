using Isotope.Data.Models;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details of a tag (for either folder or a media).
    /// </summary>
    public class TagVM
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
    }
}