using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for working with media files.
    /// </summary>
    [Route("~/@api/admin/media")]
    public class MediaController: AdminControllerBase
    {
        public MediaController(MediaManager mediaMgr)
        {
            _mediaMgr = mediaMgr;
        }

        private readonly MediaManager _mediaMgr;

        /// <summary>
        /// Displays the list of media files in a folder.
        /// </summary>
        [HttpGet, Route("")]
        public Task<IReadOnlyList<MediaThumbnailVM>> GetList([FromQuery] MediaListRequestVM request)
        {
            return _mediaMgr.GetListAsync(request);
        }

        /// <summary>
        /// Returns the details of a media file.
        /// </summary>
        [HttpGet, Route("{key}")]
        public Task<MediaVM> GetDetails(string key)
        {
            return _mediaMgr.GetAsync(key);
        }

        /// <summary>
        /// Uploads a new media file.
        /// </summary>
        [HttpPost, Route("")]
        public Task<MediaThumbnailVM> Upload([FromQuery] string folder, IFormFile file)
        {
            return _mediaMgr.UploadAsync(folder, file);
        }

        /// <summary>
        /// Updates existing media file properties.
        /// </summary>
        [HttpPut, Route("{key}")]
        public Task Update(string key, [FromBody] MediaVM vm)
        {
            return _mediaMgr.UpdateAsync(key, vm);
        }
        
        /// <summary>
        /// Retrieves current thumbnail location.
        /// </summary>
        [HttpGet, Route("{key}/thumb")]
        public Task<RectVM> GetThumbnail(string key)
        {
            return _mediaMgr.GetThumbnailAsync(key);
        }

        /// <summary>
        /// Updates thumbnail location.
        /// </summary>
        [HttpPut, Route("{key}/thumb")]
        public Task UpdateThumbnail(string key, [FromBody] RectVM rect)
        {
            return _mediaMgr.UpdateThumbnailAsync(key, rect);
        }
        
        /// <summary>
        /// Retrieves current thumbnail location.
        /// </summary>
        [HttpDelete, Route("thumb")]
        public Task Delete(string key)
        {
            return _mediaMgr.RemoveAsync(key);
        }
    }
}