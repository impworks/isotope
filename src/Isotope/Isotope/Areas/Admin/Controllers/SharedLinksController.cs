using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for working with shared links.
/// </summary>
[Route("~/@api/admin/shared-links")]
public class SharedLinksController(SharedLinkManager smMgr) : AdminControllerBase
{
    /// <summary>
    /// Returns the list of all existing shared links.
    /// </summary>
    [HttpGet, Route("")]
    public Task<IReadOnlyList<SharedLinkDetailsVM>> GetList() => smMgr.GetListAsync();

    /// <summary>
    /// Creates a new shared link.
    /// </summary>
    [HttpPost, Route("")]
    public Task<KeyResultVM> Create([FromBody] SharedLinkVM vm) => smMgr.CreateAsync(vm);

    /// <summary>
    /// Removes an existing shared link.
    /// </summary>
    [HttpDelete, Route("{key}")]
    public Task Remove(string key) => smMgr.RemoveAsync(key);

    /// <summary>
    /// Updates an existing shared link.
    /// </summary>
    [HttpPut, Route("{key}")]
    public Task Update(string key, [FromBody] SharedLinkVM vm) => smMgr.UpdateAsync(key, vm);
}