using Isotope.Areas.Front.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers
{
    /// <summary>
    /// Base controller for bootstrapping the SPA page.
    /// </summary>
    [Route("")]
    public class HomeController: FrontControllerBase
    {
        public HomeController(UserContextManager ucm)
            : base(ucm)
        {
        }
        
        /// <summary>
        /// Hack to workaround regex not matching an empty string.
        /// </summary>
        [Route("")]
        public IActionResult Index()
        {
            return File("~/@assets/front.html", "text/html");
        }

        /// <summary>
        /// Catch-all method for displaying SPA.
        /// </summary>
        [Route("{**path:regex(^(?!@))}")]
        public IActionResult Index(string path)
        {
            return File("~/@assets/front.html", "text/html");
        }
    }
}
