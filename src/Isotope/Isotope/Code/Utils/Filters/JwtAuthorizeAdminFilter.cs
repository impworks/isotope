using System.Threading.Tasks;
using Isotope.Code.Services;
using Isotope.Code.Utils.Exceptions;
using Isotope.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Code.Utils.Filters;

public class JwtAuthorizeAdminFilter(AuthService auth, AppDbContext db) : JwtAuthorizeFilter(auth)
{
    public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        await base.OnAuthorizationAsync(context);

        var userName = context.HttpContext.User?.Identity?.Name?.ToUpper();
        var user = await db.Users
                           .AsNoTracking()
                           .FirstOrDefaultAsync(x => x.NormalizedUserName == userName);
        if (user == null)
        {
            context.Result = new ObjectResult("Unauthorized") {StatusCode = 403};
            return;
        }

        if (!user.IsAdmin)
        {
            context.Result = new ObjectResult("Forbidden") {StatusCode = 403};
            return;
        }
    }
}