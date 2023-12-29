using System.Collections.Generic;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Areas.Admin.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
[Route("~/@api/admin/users")]
public class UsersController(UserManager userMgr) : AdminControllerBase
{
    /// <summary>
    /// Returns the list of all users. 
    /// </summary>
    [HttpGet, Route("")]
    public Task<IReadOnlyList<UserVM>> GetList() => userMgr.GetListAsync();

    /// <summary>
    /// Adds a new tag.
    /// </summary>
    [HttpPost, Route("")]
    public Task<UserVM> Create([FromBody] UserCreationVM vm) => userMgr.CreateAsync(vm);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    [HttpPut, Route("{id}")]
    public Task Update(string id, [FromBody] UserVM vm) => userMgr.UpdateProfileAsync(id, vm, User.Identity.Name);

    /// <summary>
    /// Updates an existing user's password.
    /// </summary>
    [HttpPut, Route("{id}/password")]
    public Task Update(string id, [FromBody] UserPasswordVM vm) => userMgr.UpdatePasswordAsync(id, vm);

    /// <summary>
    /// Removes an existing user.
    /// </summary>
    [HttpDelete, Route("{id}")]
    public Task Delete(string id) => userMgr.RemoveAsync(id, User.Identity.Name);
}