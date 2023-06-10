namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Gallery details.
    /// </summary>
    public class GalleryInfoVM
    {
        /// <summary>
        /// Returns the name of the gallery.
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// Title to display as the first entry in the breadcrumbs.
        /// </summary>
        public string Subcaption { get; set; }
        
        /// <summary>
        /// Flag indicating that guests need to authorize to view the contents.
        /// </summary>
        public bool AllowGuests { get; set; }
        
        /// <summary>
        /// Flag indicating that the current request was signed by a valid token.
        /// </summary>
        public bool IsAuthorized { get; set; }
        
        /// <summary>
        /// Flag indicating that current user has administrator privileges.
        /// </summary>
        public bool? IsAdmin { get; set; }
        
        /// <summary>
        /// Flag indicating that the share link is valid or not (if specified in request).
        /// </summary>
        public bool? IsLinkValid { get; set; }
    }
}