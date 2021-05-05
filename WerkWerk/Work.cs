using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WerkWerk
{
    public class Work<T>
    {
        private WorkPipeline<T> _pipeline = new WorkPipeline<T>();

        public string JobName { get; set; }
        public TimeSpan Interval { get; set; }
        public int MaxRetries { get; set; }

        public async Task<WorkResult> Do(WorkContext<T> context)
        {
            WorkResult result = null;
            foreach (var factory in _pipeline)
            {
                try
                {
                    var middleware = factory(context.Services);

                    context.Logger.LogInformation($"Executing job middleware: {middleware.GetType().Name}");

                    result = await middleware.Execute(context);
                    if (!result.Succeeded)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    result = WorkResult.Fail(ex.Message);
                    context.Logger.LogError(ex, "Work task failed.");
                    break;
                }
            }

            return result;
        }

        public static implicit operator bool(Work<T> work)
        {
            return !string.IsNullOrEmpty(work?.JobName);
        }

        internal Work(WorkPipeline<T> pipeline)
        {
            _pipeline = pipeline;
        }
    }
}