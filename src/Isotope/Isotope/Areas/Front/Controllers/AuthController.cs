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
    [Route("~/@api/auth")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController: Controller
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
        public async Task<LoginResponseVM> Login([FromBody] LoginRequestVM request)
        {
            return await _auth.TryLoginAsync(request);
        }
    }
}