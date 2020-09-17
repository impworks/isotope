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
        public ApiController(FolderPresenter folders, UserContextManager ucm)
        {
            _folders = folders;
            _ucm = ucm;
        }

        private readonly FolderPresenter _folders;
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
            return null;
        }

        /// <summary>
        /// Returns the list of all known tags.
        /// </summary>
        [HttpGet, Route("tags")]
        public async Task<TagVM[]> GetTags()
        {
            return null;
        }

        #region Helpers

        /// <summary>
        /// Returns the current user context.
        /// </summary>
        private async Task<UserContext> GetUserContextAsync()
        {
            return await _ucm.GetUserContextAsync(Request);
        }

        #endregion
    }
}