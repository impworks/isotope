﻿using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Base controller for bootstrapping the SPA page.
/// </summary>
[Route("~/@admin")]
public class HomeController: ControllerBase
{
    /// <summary>
    /// Hack to workaround regex not matching an empty string.
    /// </summary>
    [Route("")]
    public IActionResult Index() => File("~/@assets/admin.html", "text/html");

    /// <summary>
    /// Catch-all method for displaying SPA.
    /// </summary>
    [Route("{**path:regex(^(?!@))}")]
    public IActionResult Index(string path) => File("~/@assets/admin.html", "text/html");
}