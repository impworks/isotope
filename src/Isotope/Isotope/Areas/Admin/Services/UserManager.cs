using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Areas.Admin.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services;

/// <summary>
/// Service for managing the list of users.
/// </summary>
public class UserManager(AppDbContext db, UserManager<AppUser> userMgr, IMapper mapper)
{
    /// <summary>
    /// Returns the list of all users in the system.
    /// </summary>
    public async Task<IReadOnlyList<UserVM>> GetListAsync()
    {
        return await db.Users
                        .OrderBy(x => x.UserName)
                        .ProjectToType<UserVM>(mapper.Config)
                        .ToListAsync();
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public async Task<UserVM> CreateAsync(UserCreationVM vm)
    {
        await ValidateAsync(vm.UserInfo);
        await ValidateAsync(vm.PasswordInfo);

        var user = mapper.Map<AppUser>(vm.UserInfo);
        user.Id = Guid.NewGuid().ToString();
        var result = await userMgr.CreateAsync(user, vm.PasswordInfo.Password);
        if(!result.Succeeded)
            throw new OperationException(result.Errors.First().Description);

        return mapper.Map<UserVM>(user);
    }
        
    /// <summary>
    /// Updates the user's profile.
    /// </summary>
    public async Task UpdateProfileAsync(string id, UserVM vm, string currentUser)
    {
        var user = await FindAsync(id);
        if(!vm.IsAdmin && string.Equals(user.UserName, currentUser, StringComparison.InvariantCultureIgnoreCase))
            throw new OperationException("You cannot demote your admin rights.");
            
        mapper.Map(vm, user);
        await db.SaveChangesAsync();
    }
        
    /// <summary>
    /// Updates the user's profile.
    /// </summary>
    public async Task UpdatePasswordAsync(string id, UserPasswordVM vm)
    {
        await ValidateAsync(vm);
        var user = await FindAsync(id);
        var token = await userMgr.GeneratePasswordResetTokenAsync(user);
        await userMgr.ResetPasswordAsync(user, token, vm.Password);
    }
        
    /// <summary>
    /// Removes an existing user.
    /// </summary>
    public async Task RemoveAsync(string id, string currentUser)
    {
        var user = await FindAsync(id);
        if(string.Equals(user.UserName, currentUser, StringComparison.InvariantCultureIgnoreCase))
            throw new OperationException("You cannot remove your own account.");
            
        await userMgr.DeleteAsync(user);
    }

    /// <summary>
    /// Validates the user data. 
    /// </summary>
    private async Task ValidateAsync(UserVM vm, string id = null)
    {
        if(string.IsNullOrEmpty(vm.UserName))
            throw new OperationException("UserName cannot be empty.");

        var upper = vm.UserName.ToUpper();
        var q = db.Users.Where(x => x.NormalizedUserName == upper);
        if (id != null)
            q = q.Where(x => x.Id != id);
            
        if(await q.AnyAsync())
            throw new OperationException($"UserName '{vm.UserName}' is already taken.");
    }

    /// <summary>
    /// Validates the user password.
    /// </summary>
    private Task ValidateAsync(UserPasswordVM vm)
    {
        if(string.IsNullOrEmpty(vm.Password) || string.IsNullOrEmpty(vm.PasswordConfirmation))
            throw new OperationException("Password cannot be empty.");
            
        if(vm.Password != vm.PasswordConfirmation)
            throw new OperationException("Passwords do not match.");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the user by ID.
    /// </summary>
    private async Task<AppUser> FindAsync(string id)
    {
        return await db.Users.GetAsync(x => x.Id == id, $"User '{id}' does not exist.");
    }
}