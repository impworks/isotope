using System.IO;
using System.Threading.Tasks;
using Isotope.Areas.Front.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers;

/// <summary>
/// Base controller for bootstrapping the SPA page.
/// </summary>
[Route("")]
public class HomeController(UserContextManager ucm, OpenGraphPresenter ogp, IWebHostEnvironment env)
    : FrontControllerBase(ucm)
{
    /// <summary>
    /// Hack to workaround regex not matching an empty string.
    /// </summary>
    [Route("")]
    public Task<IActionResult> Index() => GetFrontAsync();

    /// <summary>
    /// Catch-all method for displaying SPA.
    /// </summary>
    [Route("{**path:regex(^(?!@))}")]
    public Task<IActionResult> Index(string path) => GetFrontAsync();

    /// <summary>
    /// Returns the default frontend with OG tags replaced for better sharing UX.
    /// </summary>
    private async Task<IActionResult> GetFrontAsync()
    {
        var filePath = Path.Combine(env.WebRootPath, "@assets", "front.html");
        var fileContents = await System.IO.File.ReadAllTextAsync(filePath);
        var og = await ogp.GetOpenGraphDataAsync(HttpContext);
        var processedFile = fileContents.Replace("<title>isotope</title>", og);
        return Content(processedFile, "text/html");
    }
}