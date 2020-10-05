using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for working with shared links.
    /// </summary>
    [Route("~/@api/admin/shared-links")]
    public class SharedLinksController: AdminControllerBase
    {
        public SharedLinksController(SharedLinkManagerService smls)
        {
            _smls = smls;
        }
        
        private readonly SharedLinkManagerService _smls;

        /// <summary>
        /// Returns the list of all existing shared links.
        /// </summary>
        [HttpGet, Route("")]
        public Task<IReadOnlyList<SharedLinkDetailsVM>> GetList()
        {
            return _smls.GetListAsync();
        }

        /// <summary>
        /// Creates a new shared link.
        /// </summary>
        [HttpPost, Route("")]
        public Task<KeyResultVM> Create(SharedLinkVM vm)
        {
            return _smls.CreateAsync(vm);
        }

        /// <summary>
        /// Removes an existing shared link.
        /// </summary>
        [HttpDelete, Route("{key}")]
        public Task Remove(string key)
        {
            return _smls.RemoveAsync(key);
        }
    }
}