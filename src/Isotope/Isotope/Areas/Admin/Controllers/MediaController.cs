using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for working with media files.
/// </summary>
[Route("~/@api/admin/media")]
public class MediaController(MediaManager mediaMgr) : AdminControllerBase
{
    /// <summary>
    /// Displays the list of media files in a folder.
    /// </summary>
    [HttpGet, Route("~/@api/admin/folders/{folderKey}/media")]
    public Task<IReadOnlyList<MediaThumbnailVM>> GetList(string folderKey) => mediaMgr.GetListAsync(folderKey);

    /// <summary>
    /// Updates the order of media in a folder.
    /// </summary>
    [HttpPut, Route("~/@api/admin/folders/{folderKey}/media/order")]
    public Task Reorder(string folderKey, [FromBody] string[] keys) => mediaMgr.ReorderAsync(folderKey, keys);

    /// <summary>
    /// Returns the details of a media file.
    /// </summary>
    [HttpGet, Route("{key}")]
    public Task<MediaVM> GetDetails(string key) => mediaMgr.GetAsync(key);

    /// <summary>
    /// Uploads a new media file.
    /// </summary>
    [HttpPost, Route("")]
    public Task<MediaThumbnailVM> Upload([FromQuery] string folder, IFormFile file) => mediaMgr.UploadAsync(folder, file);

    /// <summary>
    /// Updates existing media file properties.
    /// </summary>
    [HttpPut, Route("{key}")]
    public Task Update(string key, [FromBody] MediaVM vm) => mediaMgr.UpdateAsync(key, vm);

    /// <summary>
    /// Retrieves current thumbnail location.
    /// </summary>
    [HttpGet, Route("{key}/thumb")]
    public Task<RectVM> GetThumbnail(string key) => mediaMgr.GetThumbnailAsync(key);

    /// <summary>
    /// Updates thumbnail location.
    /// </summary>
    [HttpPut, Route("{key}/thumb")]
    public Task UpdateThumbnail(string key, [FromBody] RectVM rect) => mediaMgr.UpdateThumbnailAsync(key, rect);

    /// <summary>
    /// Retrieves current thumbnail location.
    /// </summary>
    [HttpDelete, Route("{key}")]
    public Task Remove(string key) => mediaMgr.RemoveAsync(new [] { key });

    /// <summary>
    /// Removes a batch of media files.
    /// </summary>
    [HttpPost, Route("mass/remove")]
    public Task Remove([FromBody] MassMediaActionVM vm) => mediaMgr.RemoveAsync(vm.Keys);

    /// <summary>
    /// Moves a batch of media files to another folder.
    /// </summary>
    [HttpPost, Route("mass/move")]
    public Task Move([FromBody] MassMediaMoveVM vm) => mediaMgr.MoveAsync(vm.FolderKey, vm.Keys);

    /// <summary>
    /// Updates a batch of media files together.
    /// </summary>
    [HttpPost, Route("mass/update")]
    public Task Update([FromBody] MassMediaUpdateVM vm) => mediaMgr.UpdateAsync(vm);
}