using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Front.Dto;

/// <summary>
/// Details about a user.
/// </summary>
public class UserInfoVM: IMapped
{
    /// <summary>
    /// Authorization token.
    /// </summary>
    public string Token { get; set; }
        
    /// <summary>
    /// Name of the user.
    /// </summary>
    public string Username { get; set; }
        
    /// <summary>
    /// Flag indicating that the user has admin privileges.
    /// </summary>
    public bool IsAdmin { get; set; }

    public void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, UserInfoVM>()
              .Map(x => x.Username, x => x.UserName)
              .Map(x => x.IsAdmin, x => x.IsAdmin)
              .Ignore(x => x.Token);
    }
}