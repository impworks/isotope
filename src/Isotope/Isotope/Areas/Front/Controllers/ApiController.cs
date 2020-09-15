using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
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
        /// <summary>
        /// Returns the entire folder tree.
        /// </summary>
        [HttpGet, Route("tree")]
        public async Task<FolderVM[]> GetTree()
        {
            return new FolderVM[0];
        }

        /// <summary>
        /// Returns the contents of a specific folder.
        /// </summary>
        [HttpGet, Route("folder")]
        public async Task<FolderContentsVM> GetFolderContents([FromQuery] FolderContentsRequestVM request)
        {
            return null;
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
    }
}