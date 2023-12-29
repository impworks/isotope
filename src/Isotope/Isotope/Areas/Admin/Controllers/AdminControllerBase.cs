using Isotope.Code.Utils.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Base class for all admin page controllers.
/// </summary>
[Area("Admin"), JwtAuthorize(true)]
public class AdminControllerBase: Controller
{
}