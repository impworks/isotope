using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
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
        /// <summary>
        /// Attempts to authorize the user.
        /// </summary> 
        [HttpPost, Route("login")]
        public async Task<LoginResponseVM> Login([FromBody] LoginRequestVM request)
        {
            return null;
        }
    }
}