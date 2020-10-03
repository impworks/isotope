using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing tags.
    /// </summary>
    [Route("~/@api/admin/tags")]
    public class TagsController: AdminControllerBase
    {
        public TagsController(TagManagerService tms)
        {
            _tms = tms;
        }
        
        private readonly TagManagerService _tms;

        /// <summary>
        /// Returns the list of all tags. 
        /// </summary>
        [HttpGet, Route("")]
        public async Task<IReadOnlyList<TagVM>> GetList()
        {
            return await _tms.GetListAsync();
        }

        /// <summary>
        /// Adds a new tag.
        /// </summary>
        [HttpPost, Route("")]
        public async Task<TagVM> Create(TagVM tag)
        {
            return await _tms.CreateAsync(tag);
        }
        
        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        [HttpPut, Route("{id:int}")]
        public async Task Update(int id, TagVM tag)
        {
            await _tms.UpdateAsync(id, tag);
        }
        
        /// <summary>
        /// Removes an existing tag.
        /// </summary>
        [HttpDelete, Route("{id:int}")]
        public async Task Delete(int id)
        {
            await _tms.RemoveAsync(id);
        }
    }
}