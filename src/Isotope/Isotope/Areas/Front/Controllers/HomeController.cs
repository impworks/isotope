using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers
{
    /// <summary>
    /// Base controller for bootstrapping the SPA page.
    /// </summary>
    [Route("")]
    public class HomeController: ControllerBase
    {
        /// <summary>
        /// Catch-all method for displayng SPA.
        /// </summary>
        [Route("{**path}")]
        public IActionResult Index()
        {
            return Ok("Frontend SPA");
        }
    }
}
