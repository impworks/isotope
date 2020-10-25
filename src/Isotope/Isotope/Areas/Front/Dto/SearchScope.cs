namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Known search modes.
    /// </summary>
    public enum SearchScope
    {
        /// <summary>
        /// Only include media from current folder.
        /// </summary>
        CurrentFolder = 1,
        
        /// <summary>
        /// Include media and subfolders from current folder and down the hierarchy.
        /// </summary>
        CurrentFolderAndSubfolders = 2,
        
        /// <summary>
        /// Include everything in the entire gallery.
        /// </summary>
        Everywhere = 3
    }
}