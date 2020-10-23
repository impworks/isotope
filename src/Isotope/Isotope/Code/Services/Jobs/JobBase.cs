using System.Threading;
using System.Threading.Tasks;

namespace Isotope.Code.Services.Jobs
{
    /// <summary>
    /// Base implementation for strongly typed jobs with arguments.
    /// </summary>
    public abstract class JobBase<TArgs>: IJob
    {
        public string GetResourceKey(object args) => GetResourceKey((TArgs) args);
        public Task ExecuteAsync(object args, CancellationToken token) => ExecuteAsync((TArgs) args, token);
        
        protected virtual string GetResourceKey(TArgs args) => null;
        protected abstract Task ExecuteAsync(TArgs args, CancellationToken token);
    }
    
    /// <summary>
    /// Base implementation for strongly typed jobs without arguments.
    /// </summary>
    public abstract class JobBase: IJob
    {
        public string GetResourceKey(object args) => GetResourceKey();
        public Task ExecuteAsync(object args, CancellationToken token) => ExecuteAsync(token);
        
        protected virtual string GetResourceKey() => null;
        protected abstract Task ExecuteAsync(CancellationToken token);
    }
}