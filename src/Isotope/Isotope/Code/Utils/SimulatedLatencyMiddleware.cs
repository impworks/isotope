using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Isotope.Code.Utils;

/// <summary>
/// Middleware for simulating slow internet connection (for debugging preloaders).
/// </summary>
public class SimulatedLatencyMiddleware(RequestDelegate next, int latency)
{
    [UsedImplicitly]
    public async Task Invoke(HttpContext context)
    {
        await Task.Delay(latency);
        await next(context);
    }
}