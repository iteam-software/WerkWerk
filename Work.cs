using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WerkWerk
{
    public class Work
    {
        private WorkPipeline _pipeline = new WorkPipeline();

        public string JobName { get; set; }
        public TimeSpan Interval { get; set; }
        public int MaxRetries { get; set; }

        public async Task<WorkResult> Do(WorkContext context)
        {
            WorkResult result = null;
            foreach (var factory in _pipeline)
            {
                try
                {
                    var middleware = factory(context.Services);
                    result = await middleware.Execute(context);
                    if (!result.Succeeded)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogError(ex, result.Error);
                    break;
                }
            }

            return result;
        }

        public static implicit operator bool(Work work)
        {
            return !string.IsNullOrEmpty(work?.JobName);
        }

        internal Work(WorkPipeline pipeline)
        {
            _pipeline = pipeline;
        }
    }
}