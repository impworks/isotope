namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Result of an authorization.
    /// </summary>
    public class LoginResponseVM
    {
        /// <summary>
        /// Flag indicating that the authorization was successful.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Details if the user (if success = true).
        /// </summary>
        public UserInfoVM User { get; set; }
        
        /// <summary>
        /// Authorization error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}