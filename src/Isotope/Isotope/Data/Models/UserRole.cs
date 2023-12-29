namespace Isotope.Data.Models;

/// <summary>
/// Known user roles.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Basic user that can view the entire gallery.
    /// </summary>
    User = 1,
        
    /// <summary>
    /// Advanced user with administration privileges.
    /// </summary>
    Admin = 2
}