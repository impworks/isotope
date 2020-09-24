using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Filter for handling\logging common exceptions.
    /// </summary>
    public class CommonExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public CommonExceptionFilterAttribute(ILogger logger)
        {
            _logger = logger;
        }
        
        private readonly ILogger _logger;

        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;

            if (context.Exception is NotFoundException nfe)
            {
                context.Result = new ObjectResult(nfe.Message) { StatusCode = 404 };
                return;
            }

            if (context.Exception is NotAllowedException nae)
            {
                context.Result = new ObjectResult(nae.Message) { StatusCode = 403 };
                return;
            }
            
            _logger.Error(context.Exception.Demystify(), "Unhandled exception at " + context.ActionDescriptor.DisplayName);
            context.Result = new ObjectResult("Unknown error") { StatusCode = 500 };
        }
    }
}