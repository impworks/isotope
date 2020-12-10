using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Middleware for simulating slow internet connection (for debugging preloaders).
    /// </summary>
    public class SimulatedLatencyMiddleware
    {
        public SimulatedLatencyMiddleware(RequestDelegate next, int latency)
        {
            _next = next;
            _latency = latency;
        }
        
        private readonly RequestDelegate _next;
        private readonly int _latency;

        [UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            await Task.Delay(_latency);
            await _next(context);
        }
    }
}