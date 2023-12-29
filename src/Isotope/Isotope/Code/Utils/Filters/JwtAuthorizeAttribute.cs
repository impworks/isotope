using System;
using Microsoft.AspNetCore.Mvc;

namespace Isotope.Code.Utils.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JwtAuthorizeAttribute(bool isAdmin = false) : TypeFilterAttribute(isAdmin ? typeof(JwtAuthorizeAdminFilter) : typeof(JwtAuthorizeFilter));