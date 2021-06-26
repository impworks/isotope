namespace Isotope.Areas.Front.Dto
{
    /// <summary>
    /// Request for changing a user's own password.
    /// </summary>
    public class ChangePasswordRequestVM
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordRepeat { get; set; }
    }
}