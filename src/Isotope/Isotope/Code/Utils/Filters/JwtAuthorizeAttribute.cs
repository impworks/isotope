using System;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Code.Utils.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute: TypeFilterAttribute
    {
        public JwtAuthorizeAttribute()
            : base(typeof(JwtAuthorizeFilter))
        {
        }
    }
}