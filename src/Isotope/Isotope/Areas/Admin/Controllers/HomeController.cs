using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Base controller for bootstrapping the SPA page.
    /// </summary>
    [Route("~/@admin")]
    public class HomeController: ControllerBase
    {
        /// <summary>
        /// Catch-all method for displaying SPA.
        /// </summary>
        [Route("{**path}")]
        public IActionResult Index()
        {
            return Ok("Admin SPA");
        }
    }
}
