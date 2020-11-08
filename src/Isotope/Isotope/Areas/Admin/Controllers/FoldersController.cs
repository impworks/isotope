using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for working with folders.
    /// </summary>
    [Route("~/@api/admin/folders")]
    public class FoldersController: AdminControllerBase
    {
        public FoldersController(FolderManager folderMgr)
        {
            _folderMgr = folderMgr;
        }
        
        private readonly FolderManager _folderMgr;

        /// <summary>
        /// Returns the tree of all folders in the system.
        /// </summary>
        [HttpGet, Route("")]
        public Task<IReadOnlyList<FolderTitleVM>> GetTree()
        {
            return _folderMgr.GetTreeAsync();
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        [HttpPost, Route("{key?}")]
        public Task<FolderTitleVM> Create(string key, [FromBody] FolderVM vm)
        {
            return _folderMgr.CreateAsync(key, vm);
        }

        /// <summary>
        /// Returns the folder info for editing.
        /// </summary>
        [HttpGet, Route("{key}")]
        public Task<FolderVM> GetDetails(string key)
        {
            return _folderMgr.GetAsync(key);
        }

        /// <summary>
        /// Updates an existing folder.
        /// </summary>
        [HttpPut, Route("{key}")]
        public Task Update(string key, [FromBody] FolderVM vm)
        {
            return _folderMgr.UpdateAsync(key, vm);
        }

        /// <summary>
        /// Removes an existing folder with all its contents.
        /// </summary>
        [HttpDelete, Route("{key}")]
        public Task Delete(string key)
        {
            return _folderMgr.RemoveAsync(key);
        }
    }
}