using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Dto;
using Isotope.Data;
using Isotope.Data.Models;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Admin.Services
{
    /// <summary>
    /// Service for managing the list of users.
    /// </summary>
    public class UserManagerService
    {
        public UserManagerService(AppDbContext db, UserManager<AppUser> userMgr, IMapper mapper)
        {
            _db = db;
            _userMgr = userMgr;
            _mapper = mapper;
        }

        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userMgr;
        private readonly IMapper _mapper;

        /// <summary>
        /// Returns the list of all users in the system.
        /// </summary>
        public async Task<IReadOnlyList<UserVM>> GetListAsync()
        {
            return await _db.Users
                            .OrderBy(x => x.UserName)
                            .ProjectToType<UserVM>(_mapper.Config)
                            .ToListAsync();
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        public async Task<UserVM> CreateAsync(UserCreationVM user)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Updates the user's profile.
        /// </summary>
        public async Task UpdateProfileAsync(string id, UserVM user)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Updates the user's profile.
        /// </summary>
        public async Task UpdatePasswordAsync(string id, UserPasswordVM pass)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Removes an existing user.
        /// </summary>
        public async Task RemoveAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}