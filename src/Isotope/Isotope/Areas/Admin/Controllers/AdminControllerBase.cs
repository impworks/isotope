using Isotope.Code.Utils;
using Isotope.Code.Utils.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Base class for all admin page controllers.
    /// </summary>
    [Area("Admin"), JwtAuthorize]
    public class AdminControllerBase: Controller
    {
        
    }
}