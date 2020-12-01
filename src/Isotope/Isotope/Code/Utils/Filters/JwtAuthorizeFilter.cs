using System;
using System.Threading.Tasks;
using Isotope.Code.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Isotope.Code.Utils.Filters
{
    /// <summary>
    /// Retrieves authorization info from the bearer token, if one is specified.
    /// </summary>
    public class JwtAuthorizeFilter: IAsyncAuthorizationFilter
    {
        public JwtAuthorizeFilter(AuthService auth)
        {
            _auth = auth;
        }
        
        protected readonly AuthService _auth;
        
        public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            const string Scheme = "Bearer ";
            var auth = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (auth?.StartsWith(Scheme, StringComparison.OrdinalIgnoreCase) != true)
                return;

            var token = auth.Substring(Scheme.Length).Trim();
            context.HttpContext.User = _auth.ValidateToken(token);
        }
    }
}