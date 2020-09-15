namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Request for filtering media files.
    /// </summary>
    public class FolderContentsRequestVM
    {
        /// <summary>
        /// Root folder to search in.
        /// </summary>
        public string Folder { get; set; }
        
        /// <summary>
        /// List of required tags (or empty to display everything).
        /// </summary>
        public int[] Tags { get; set; }
        
        /// <summary>
        /// Earliest date in the range.
        /// </summary>
        public string DateFrom { get; set; }
        
        /// <summary>
        /// Latest date in the range.
        /// </summary>
        public string DateTo { get; set; }
    }
}