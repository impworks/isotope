using System.Threading;
using System.Threading.Tasks;

namespace Isotope.Code.Services.Jobs
{
    /// <summary>
    /// Base implementation for strongly typed jobs.
    /// </summary>
    public abstract class JobBase<TArgs>: IJob
    {
        public string GetResourceKey(object args) => GetResourceKey((TArgs) args);
        public Task ExecuteAsync(object args, CancellationToken token) => ExecuteAsync((TArgs) args, token);
        
        protected abstract string GetResourceKey(TArgs args);
        protected abstract Task ExecuteAsync(TArgs args, CancellationToken token);
    }
}