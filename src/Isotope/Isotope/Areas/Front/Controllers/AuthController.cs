using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Areas.Front.Services;
using Isotope.Code.Services;
using Isotope.Code.Utils.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers;

/// <summary>
/// Controller for authorization methods.
/// </summary>
[Route("~/@api/auth"), ApiController]
public class AuthController(AuthService auth, UserContextManager ucm) : FrontControllerBase(ucm)
{
    /// <summary>
    /// Attempts to authorize the user.
    /// </summary> 
    [HttpPost, Route("login")]
    [AllowAnonymous]
    public Task<LoginResponseVM> Login([FromBody] LoginRequestVM request)
    {
        var result = auth.TryLoginAsync(request);
        ClearCookie();
        return result;
    }

    /// <summary>
    /// Changes the password for a user.
    /// </summary>
    [HttpPost, Route("change-password")]
    [JwtAuthorize]
    public async Task ChangePassword([FromBody] ChangePasswordRequestVM request) => await auth.ChangePasswordAsync(request, await GetUserContextAsync());

    /// <summary>
    /// Removes the auth cookie.
    /// </summary>
    private void ClearCookie()
    {
        // hack: SignInManager always implicitly sets cookies on successful authorization
        // so we need to explicitly clear them from the response in order to properly use JWT authorization
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
    }
}