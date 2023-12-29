using System;
using System.Threading.Tasks;
using Isotope.Code.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Isotope.Code.Utils.Filters;

/// <summary>
/// Retrieves authorization info from the bearer token, if one is specified.
/// </summary>
public class JwtAuthorizeFilter(AuthService auth) : IAsyncAuthorizationFilter
{
    public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        const string Scheme = "Bearer ";
        var authValue = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (authValue.StartsWith(Scheme, StringComparison.OrdinalIgnoreCase) != true)
            return;

        var token = authValue.Substring(Scheme.Length).Trim();
        context.HttpContext.User = auth.ValidateToken(token);
    }
}