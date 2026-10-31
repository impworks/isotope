using System;
using Isotope.Code.Services.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Isotope.Code.Utils.Filters;

/// <summary>
/// Raises the request body size limit for upload endpoints to the configured WebServer.MaxUploadSize value.
/// Other endpoints keep the server's default limit.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UploadSizeLimitAttribute() : TypeFilterAttribute(typeof(UploadSizeLimitFilter))
{
    /// <summary>
    /// Limit used when no value is configured (100 MB).
    /// </summary>
    public const long DefaultMaxUploadSize = 100 * 1024 * 1024;

    private class UploadSizeLimitFilter(StaticConfig config) : IResourceFilter, IOrderedFilter
    {
        // must run before model binding reads the form
        public int Order => -1000;

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var limit = config.WebServer?.MaxUploadSize ?? DefaultMaxUploadSize;
            var http = context.HttpContext;

            var bodySizeFeature = http.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (bodySizeFeature is { IsReadOnly: false })
                bodySizeFeature.MaxRequestBodySize = limit;

            if (http.Request.HasFormContentType && http.Features.Get<IFormFeature>()?.Form == null)
            {
                var formOptions = new FormOptions { MultipartBodyLengthLimit = limit };
                http.Features.Set<IFormFeature>(new FormFeature(http.Request, formOptions));
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
