using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for retrieving gallery statistics.
/// </summary>
[Route("~/@api/admin/stats")]
public class StatsController(StatsManager statsMgr) : AdminControllerBase
{
    /// <summary>
    /// Returns gallery statistics.
    /// </summary>
    [HttpGet, Route("")]
    public Task<StatsVM> Get() => statsMgr.GetAsync();
}
