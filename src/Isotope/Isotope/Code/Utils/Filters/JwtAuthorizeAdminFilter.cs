using System.Threading.Tasks;
using Isotope.Code.Services;
using Isotope.Code.Utils.Exceptions;
using Isotope.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Code.Utils.Filters
{
    public class JwtAuthorizeAdminFilter: JwtAuthorizeFilter
    {
        public JwtAuthorizeAdminFilter(AuthService auth, AppDbContext db)
            : base(auth)
        {
            _db = db;
        }
        
        private readonly AppDbContext _db;

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await base.OnAuthorizationAsync(context);

            var userName = context.HttpContext.User?.Identity?.Name?.ToUpper();
            var user = await _db.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.NormalizedUserName == userName);
            if (user == null)
                throw new NotAllowedException("Unauthorized");
            
            if(!user.IsAdmin)
                throw new NotAllowedException("Forbidden");
        }
    }
}