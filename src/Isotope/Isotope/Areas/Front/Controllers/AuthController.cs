using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers
{
    /// <summary>
    /// Controller for authorization methods.
    /// </summary>
    [Route("~/@api/auth"), AllowAnonymous, ApiController]
    public class AuthController: FrontControllerBase
    {
        public AuthController(AuthService auth)
        {
            _auth = auth;
        }
        
        private readonly AuthService _auth;
        
        /// <summary>
        /// Attempts to authorize the user.
        /// </summary> 
        [HttpPost, Route("login")]
        public Task<LoginResponseVM> Login([FromBody] LoginRequestVM request)
        {
            var result = _auth.TryLoginAsync(request);
            ClearCookie();
            return result;
        }

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
}