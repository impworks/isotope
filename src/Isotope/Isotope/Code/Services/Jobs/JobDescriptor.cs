using System.Threading;

namespace Isotope.Code.Services.Jobs
{
    public class JobDescriptor
    {
        public IJob Job { get; set; }
        public object Arguments { get; set; }
        
        public string ResourceKey { get; set; }
        
        public CancellationTokenSource Cancellation { get; set; }
        public int JobStateId { get; set; }

        public override string ToString()
        {
            return $"{JobStateId} ({Job?.GetType().Name ?? "Unknown"})";
        }
    }
}