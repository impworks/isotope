using Isotope.Data.Models;

namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Context of a user's invocation.
    /// </summary>
    public class UserContext
    {
        /// <summary>
        /// Currently authorized user.
        /// </summary>
        public AppUser User { get; set; }
        
        /// <summary>
        /// Link ID (may be invalid).
        /// </summary>
        public string LinkId { get; set; }
        
        /// <summary>
        /// Link associated with the current request.
        /// </summary>
        public SharedLink Link { get; set; }
    }
}