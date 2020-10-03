using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Base class for all admin page controllers.
    /// </summary>
    [Area("Admin"), Authorize]
    public class AdminControllerBase: Controller
    {
        
    }
}