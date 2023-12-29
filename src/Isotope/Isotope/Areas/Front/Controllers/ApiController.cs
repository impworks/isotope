using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Areas.Front.Services;
using Isotope.Code.Utils.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers;

/// <summary>
/// General controller for frontend API methods.
/// </summary>
[Route("~/@api"), ApiController]
[JwtAuthorize]
public class ApiController(GalleryInfoPresenter info, FolderPresenter folders, TagsPresenter tags, MediaPresenter media, UserContextManager ucm)
    : FrontControllerBase(ucm)
{
    /// <summary>
    /// Returns basic info about the gallery.
    /// </summary>
    [HttpGet, Route("info")]
    public async Task<GalleryInfoVM> GetInfo() => await info.GetInfoAsync(await GetUserContextAsync(checkAuth: false));

    /// <summary>
    /// Returns the entire folder tree.
    /// </summary>
    [HttpGet, Route("tree")]
    public async Task<FolderVM[]> GetTree() => await folders.GetFolderTreeAsync(await GetUserContextAsync());

    /// <summary>
    /// Returns the contents of a specific folder.
    /// </summary>
    [HttpGet, Route("folder")]
    public async Task<FolderContentsVM> GetFolderContents([FromQuery] FolderContentsRequestVM request) => await folders.GetFolderContentsAsync(request, await GetUserContextAsync());

    /// <summary>
    /// Returns the media details.
    /// </summary>
    [HttpGet, Route("media")]
    public async Task<MediaVM> GetMedia([FromQuery] string key) => await media.GetMediaAsync(key, await GetUserContextAsync());

    /// <summary>
    /// Returns the list of all known tags.
    /// </summary>
    [HttpGet, Route("tags")]
    public async Task<TagVM[]> GetTags() => await tags.GetTagsAsync(await GetUserContextAsync());
}