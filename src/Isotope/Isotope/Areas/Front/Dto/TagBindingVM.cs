using Isotope.Data.Models;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details of a tag related to a folder.
    /// </summary>
    public class TagBindingVM
    {
        /// <summary>
        /// Related tag.
        /// </summary>
        public TagVM Tag { get; set; }
        
        /// <summary>
        /// Location of the tagged entity.
        /// </summary>
        public TagBindingLocationVM Location { get; set; }
        
        /// <summary>
        /// Type of the binding.
        /// </summary>
        public TagBindingType Type { get; set; }
    }
}