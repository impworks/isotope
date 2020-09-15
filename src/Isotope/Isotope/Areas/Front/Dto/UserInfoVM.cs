namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Details about a user.
    /// </summary>
    public class UserInfoVM
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Flag indicating that the user has admin privileges.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}