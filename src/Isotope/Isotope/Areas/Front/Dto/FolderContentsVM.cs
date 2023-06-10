namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// The contents of a folder.
    /// </summary>
    public class FolderContentsVM
    {
        /// <summary>
        /// Readable name of the folder.
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// Related tags.
        /// </summary>
        public TagBindingVM[] Tags { get; set; }
        
        /// <summary>
        /// Media in the folder.
        /// </summary>
        public MediaThumbnailVM[] Media { get; set; }
        
        /// <summary>
        /// Folders in current folder (1 level of depth inside).
        /// </summary>
        public FolderVM[] Subfolders { get; set; }
    }
}