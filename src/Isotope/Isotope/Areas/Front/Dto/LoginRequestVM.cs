namespace Isotope.Areas.Front.Dto;

/// <summary>
/// Request for authorization.
/// </summary>
public class LoginRequestVM
{
    /// <summary>
    /// Name of the user's account.
    /// </summary>
    public string Username { get; set; }
        
    /// <summary>
    /// Password of the user's account.
    /// </summary>
    public string Password { get; set; }
}