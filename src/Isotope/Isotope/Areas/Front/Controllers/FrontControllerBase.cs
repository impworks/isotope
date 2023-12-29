using System.Threading.Tasks;
using Isotope.Areas.Front.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers;

/// <summary>
/// Base class for public API controllers.
/// </summary>
[Area("Front")]
public abstract class FrontControllerBase(UserContextManager ucm) : ControllerBase
{
    /// <summary>
    /// Returns the current user context.
    /// </summary>
    protected async Task<UserContext> GetUserContextAsync(bool checkAuth = true) => await ucm.GetUserContextAsync(HttpContext, checkAuth);
}