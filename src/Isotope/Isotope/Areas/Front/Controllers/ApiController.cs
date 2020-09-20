using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Areas.Front.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Front.Controllers
{
    /// <summary>
    /// General controller for frontend API methods.
    /// </summary>
    [Route("~/@api")]
    [ApiController]
    public class ApiController: ControllerBase
    {
        public ApiController(FolderPresenter folders, TagsPresenter tags, MediaPresenter media, UserContextManager ucm)
        {
            _folders = folders;
            _tags = tags;
            _media = media;
            _ucm = ucm;
        }

        private readonly FolderPresenter _folders;
        private readonly TagsPresenter _tags;
        private readonly MediaPresenter _media;
        private readonly UserContextManager _ucm;
        
        /// <summary>
        /// Returns the entire folder tree.
        /// </summary>
        [HttpGet, Route("tree")]
        public async Task<FolderVM[]> GetTree()
        {
            return await _folders.GetFolderTreeAsync(await GetUserContextAsync());
        }

        /// <summary>
        /// Returns the contents of a specific folder.
        /// </summary>
        [HttpGet, Route("folder")]
        public async Task<FolderContentsVM> GetFolderContents([FromQuery] FolderContentsRequestVM request)
        {
            return await _folders.GetFolderContentsAsync(request, await GetUserContextAsync());
        }

        /// <summary>
        /// Returns the media details.
        /// </summary>
        [HttpGet, Route("media")]
        public async Task<MediaVM> GetMedia([FromQuery] string key)
        {
            return await _media.GetMediaAsync(key, await GetUserContextAsync());
        }

        /// <summary>
        /// Returns the list of all known tags.
        /// </summary>
        [HttpGet, Route("tags")]
        public async Task<TagVM[]> GetTags()
        {
            return await _tags.GetTagsAsync(await GetUserContextAsync());
        }

        #region Helpers

        /// <summary>
        /// Returns the current user context.
        /// </summary>
        private async Task<UserContext> GetUserContextAsync()
        {
            return await _ucm.GetUserContextAsync(HttpContext);
        }

        #endregion
    }
}