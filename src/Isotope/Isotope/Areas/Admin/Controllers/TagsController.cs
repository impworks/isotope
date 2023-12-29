using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for managing tags.
/// </summary>
[Route("~/@api/admin/tags")]
public class TagsController(TagManager tagMgr) : AdminControllerBase
{
    /// <summary>
    /// Returns the list of all tags. 
    /// </summary>
    [HttpGet, Route("")]
    public Task<IReadOnlyList<TagVM>> GetList() => tagMgr.GetListAsync();

    /// <summary>
    /// Adds a new tag.
    /// </summary>
    [HttpPost, Route("")]
    public Task<TagVM> Create([FromBody] TagVM tag) => tagMgr.CreateAsync(tag);

    /// <summary>
    /// Updates an existing tag.
    /// </summary>
    [HttpPut, Route("{id:int}")]
    public Task Update(int id, [FromBody] TagVM tag) => tagMgr.UpdateAsync(id, tag);

    /// <summary>
    /// Removes an existing tag.
    /// </summary>
    [HttpDelete, Route("{id:int}")]
    public Task Delete(int id) => tagMgr.RemoveAsync(id);
}