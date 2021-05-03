using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WerkWerk
{
    public class WorkBuilder
    {
        private WorkPipeline _pipeline = new WorkPipeline();
        private string _name;
        private TimeSpan _interval;
        private int _maxRetries;

        public WorkBuilder Setup(string name, TimeSpan interval, int maxRetries = 3)
        {
            _name = name;
            _interval = interval;
            _maxRetries = maxRetries;
            return this;
        }

        public WorkBuilder Use(Func<WorkContext, Task<WorkResult>> runner)
        {
            _pipeline.Enqueue(provider => new WorkMiddleware(runner));
            return this;
        }

        public WorkBuilder Use<TMiddleware>() where TMiddleware : IWorkMiddleware
        {
            _pipeline.Enqueue(provider => provider.GetRequiredService<TMiddleware>());
            return this;
        }

        internal Work Build()
        {
            return new Work(_pipeline)
            {
                JobName = _name,
                Interval = _interval,
                MaxRetries = _maxRetries,
            };
        }

        internal WorkBuilder() { }
    }
}