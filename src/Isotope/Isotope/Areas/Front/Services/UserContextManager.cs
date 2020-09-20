using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Services.Config;
using Isotope.Code.Utils;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Areas.Front.Services
{
    /// <summary>
    /// Helper service for retrieving user context from a request.
    /// </summary>
    public class UserContextManager
    {
        public UserContextManager(AppDbContext db, ConfigService config)
        {
            _db = db;
            _config = config;
        }

        private readonly AppDbContext _db;
        private readonly ConfigService _config;

        /// <summary>
        /// Returns the user context from current request.
        /// </summary>
        public async Task<UserContext> GetUserContextAsync(HttpContext ctx)
        {
            var link = await GetSharedLinkAsync(ctx.Request.Query["sh"].ToString());
            var user = await GetUserAsync(ctx.User.Identity.Name);

            if (_config.GetDynamicConfig().AllowGuests == false && link == null && user == null)
                throw new NotAllowedException("Unauthorized");

            return new UserContext
            {
                User = user,
                Link = link
            };
        }

        /// <summary>
        /// Returns a user by username.
        /// </summary>
        private async Task<AppUser> GetUserAsync(string username)
        {
            return await _db.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.UserName == username);
        }
        
        /// <summary>
        /// Finds the link by ID.
        /// </summary>
        private async Task<SharedLink> GetSharedLinkAsync(string linkKey)
        {
            if (string.IsNullOrEmpty(linkKey))
                return null;

            var link = await _db.SharedLinks
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == linkKey);
            
            if(link == null)
                throw new NotAllowedException($"Link ({linkKey}) not found.");

            return link;
        }
    }
}