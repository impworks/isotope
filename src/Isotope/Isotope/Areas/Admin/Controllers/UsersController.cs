using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [Route("~/@api/admin/users")]
    public class UsersController: AdminControllerBase
    {
        public UsersController(UserManager userMgr)
        {
            _userMgr = userMgr;
        }
        
        private readonly UserManager _userMgr;

        /// <summary>
        /// Returns the list of all users. 
        /// </summary>
        [HttpGet, Route("")]
        public Task<IReadOnlyList<UserVM>> GetList()
        {
            return _userMgr.GetListAsync();
        }

        /// <summary>
        /// Adds a new tag.
        /// </summary>
        [HttpPost, Route("")]
        public Task<UserVM> Create([FromBody] UserCreationVM vm)
        {
            return _userMgr.CreateAsync(vm);
        }
        
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        [HttpPut, Route("{id}")]
        public Task Update(string id, [FromBody] UserVM vm)
        {
            return _userMgr.UpdateProfileAsync(id, vm);
        }
        
        /// <summary>
        /// Updates an existing user's password.
        /// </summary>
        [HttpPut, Route("{id}/password")]
        public Task Update(string id, [FromBody] UserPasswordVM vm)
        {
            return _userMgr.UpdatePasswordAsync(id, vm);
        }
        
        /// <summary>
        /// Removes an existing user.
        /// </summary>
        [HttpDelete, Route("{id}")]
        public Task Delete(string id)
        {
            return _userMgr.RemoveAsync(id);
        }
    }
}