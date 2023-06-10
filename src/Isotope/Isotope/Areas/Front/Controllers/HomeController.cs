using System.IO;
using System.Threading.Tasks;
using Isotope.Areas.Front.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers
{
    /// <summary>
    /// Base controller for bootstrapping the SPA page.
    /// </summary>
    [Route("")]
    public class HomeController: FrontControllerBase
    {
        private readonly OpenGraphPresenter _ogp;
        private readonly IWebHostEnvironment _env;

        public HomeController(UserContextManager ucm, OpenGraphPresenter ogp, IWebHostEnvironment env)
            : base(ucm)
        {
            _ogp = ogp;
            _env = env;
        }
        
        /// <summary>
        /// Hack to workaround regex not matching an empty string.
        /// </summary>
        [Route("")]
        public Task<IActionResult> Index()
        {
            return GetFrontAsync();
        }

        /// <summary>
        /// Catch-all method for displaying SPA.
        /// </summary>
        [Route("{**path:regex(^(?!@))}")]
        public Task<IActionResult> Index(string path)
        {
            return GetFrontAsync();
        }

        /// <summary>
        /// Returns the default frontend with OG tags replaced for better sharing UX.
        /// </summary>
        private async Task<IActionResult> GetFrontAsync()
        {
            var filePath = Path.Combine(_env.WebRootPath, "@assets", "front.html");
            var fileContents = await System.IO.File.ReadAllTextAsync(filePath);
            var og = await _ogp.GetOpenGraphDataAsync(HttpContext);
            var processedFile = fileContents.Replace("<!-- *OG PLACEHOLDER* -->", og);
            return Content(processedFile, "text/html");
        }
    }
}
