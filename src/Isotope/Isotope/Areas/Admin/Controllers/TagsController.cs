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
        public TagsController(TagManager tagMgr)
        {
            _tagMgr = tagMgr;
        }
        
        private readonly TagManager _tagMgr;

        /// <summary>
        /// Returns the list of all tags. 
        /// </summary>
        [HttpGet, Route("")]
        public Task<IReadOnlyList<TagVM>> GetList()
        {
            return _tagMgr.GetListAsync();
        }

        /// <summary>
        /// Adds a new tag.
        /// </summary>
        [HttpPost, Route("")]
        public Task<TagVM> Create(TagVM tag)
        {
            return _tagMgr.CreateAsync(tag);
        }
        
        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        [HttpPut, Route("{id:int}")]
        public Task Update(int id, TagVM tag)
        {
            return _tagMgr.UpdateAsync(id, tag);
        }
        
        /// <summary>
        /// Removes an existing tag.
        /// </summary>
        [HttpDelete, Route("{id:int}")]
        public Task Delete(int id)
        {
            return _tagMgr.RemoveAsync(id);
        }
    }
}