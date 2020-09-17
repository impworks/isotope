using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Data;
using Microsoft.AspNetCore.Http;

namespace Isotope.Areas.Front.Services
{
    /// <summary>
    /// Helper service for retrieving user context from a request.
    /// </summary>
    public class UserContextManager
    {
        public UserContextManager(AppDbContext db)
        {
            _db = db;
        }

        private readonly AppDbContext _db;

        /// <summary>
        /// Returns the user context from current request.
        /// </summary>
        public async Task<UserContext> GetUserContextAsync(HttpRequest request)
        {
            // todo
            return new UserContext();
        }
    }
}