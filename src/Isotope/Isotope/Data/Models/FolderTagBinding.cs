namespace Isotope.Data.Models
{
    /// <summary>
    /// Binding of a tag to a folder.
    /// </summary>
    public class FolderTagBinding: TagBinding
    {
        /// <summary>
        /// Related folder.
        /// </summary>
        public Folder Folder { get; set; }
    }
}
