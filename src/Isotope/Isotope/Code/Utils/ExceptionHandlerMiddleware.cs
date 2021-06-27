using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Utils;
using Isotope.Code.Utils.Exceptions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Middleware for reporting errors in a civilized fashion.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        [UsedImplicitly]
        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException nfe)
                    await ErrorAsync(ctx, 404, nfe.Message);
                else if(ex is NotAllowedException nae)
                    await ErrorAsync(ctx, 403, nae.Message);
                else if (ex is OperationException oe)
                    await ErrorAsync(ctx, 400, oe.Message);
                else
                {
                    _logger.Error(ex.Demystify(), "Unhandled exception in " + ctx.Request.GetDisplayUrl());
                    await ErrorAsync(ctx, 500, "Unknown error");
                }
            }
        }

        /// <summary>
        /// Writes the response to response.
        /// </summary>
        private async Task ErrorAsync(HttpContext ctx, int httpCode, object data)
        {
            ctx.Response.StatusCode = httpCode;
            ctx.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new { error = data });
            await ctx.Response.WriteAsync(result);
        }
    }
}
