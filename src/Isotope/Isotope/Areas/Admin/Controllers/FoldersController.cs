using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for working with folders.
/// </summary>
[Route("~/@api/admin/folders")]
public class FoldersController(FolderManager folderMgr) : AdminControllerBase
{
    /// <summary>
    /// Returns the tree of folders starting from the given key or root.
    /// </summary>
    [HttpGet, Route("")]
    public Task<IReadOnlyList<FolderTitleVM>> GetTree([FromQuery] string rootKey = null) => folderMgr.GetTreeAsync(rootKey);
    
    /// <summary>
    /// Creates a new folder.
    /// </summary>
    [HttpPost, Route("{key?}")]
    public Task<FolderTitleVM> Create(string key, [FromBody] FolderVM vm) => folderMgr.CreateAsync(key, vm);

    /// <summary>
    /// Returns the folder info for editing.
    /// </summary>
    [HttpGet, Route("{key}")]
    public Task<FolderVM> GetDetails(string key) => folderMgr.GetAsync(key);

    /// <summary>
    /// Updates an existing folder.
    /// </summary>
    [HttpPut, Route("{key}")]
    public Task Update(string key, [FromBody] FolderVM vm) => folderMgr.UpdateAsync(key, vm);

    /// <summary>
    /// Removes an existing folder with all its contents.
    /// </summary>
    [HttpDelete, Route("{key}")]
    public Task Delete(string key) => folderMgr.RemoveAsync(key);

    /// <summary>
    /// Moves a folder to another location.
    /// </summary>
    [HttpPost, Route("move")]
    public Task Move([FromBody] MoveFolderVM vm) => folderMgr.MoveAsync(vm.SourceKey, vm.TargetKey);
}