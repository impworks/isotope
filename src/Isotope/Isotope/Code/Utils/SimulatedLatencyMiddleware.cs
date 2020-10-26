using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Isotope.Code.Utils
{
    public class SimulatedLatencyMiddleware
    {
        public SimulatedLatencyMiddleware(RequestDelegate next, int latency)
        {
            _next = next;
            _latency = latency;
        }
        
        private readonly RequestDelegate _next;
        private readonly int _latency;

        public async Task Invoke(HttpContext context)
        {
            await Task.Delay(_latency);
            await _next(context);
        }
    }
}