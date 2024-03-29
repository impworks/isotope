using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for working with shared links.
/// </summary>
[Route("~/@api/admin/config")]
public class ConfigController(ConfigManager cfgMgr) : AdminControllerBase
{
    /// <summary>
    /// Returns the current configuration state.
    /// </summary>
    [HttpGet, Route("")]
    public Task<ConfigVM> Get() => cfgMgr.GetAsync();

    /// <summary>
    /// Updates the configuration state.
    /// </summary>
    [HttpPut, Route("")]
    public Task Set([FromBody] ConfigVM vm) => cfgMgr.SetAsync(vm);
}