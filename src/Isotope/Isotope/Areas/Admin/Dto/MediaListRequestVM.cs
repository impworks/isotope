namespace Isotope.Areas.Admin.Dto
{
    public class MediaListRequestVM
    {
        /// <summary>
        /// Folder key.
        /// </summary>
        public string Folder { get; set; }
        
        /// <summary>
        /// Orderable field.
        /// </summary>
        public string OrderBy { get; set; }
        
        /// <summary>
        /// Order direction.
        /// </summary>
        public bool OrderDesc { get; set; }
        
        /// <summary>
        /// Page (0-based).
        /// </summary>
        public int Page { get; set; }
    }
}