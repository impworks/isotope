namespace Isotope.Data.Models
{
    /// <summary>
    /// Types of tags.
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// Person, either depicted or taking the photo.
        /// </summary>
        Person = 1,

        /// <summary>
        /// Place where the media was created.
        /// </summary>
        Location = 2,

        /// <summary>
        /// Animal on a photo.
        /// </summary>
        Pet = 3,

        /// <summary>
        /// Other attributes (category, etc).
        /// </summary>
        Custom = 4
    }
}
