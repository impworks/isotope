using Microsoft.AspNetCore.Mvc.Filters;

namespace Isotope.Code.Utils
{
    public class TryAuthorizeFilter: IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // do nothing! just force the pipeline to populate httpcontext.user
        }
    }
}