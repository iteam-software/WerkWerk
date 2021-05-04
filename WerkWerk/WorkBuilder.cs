using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WerkWerk
{
    public class WorkBuilder<T>
    {
        private WorkPipeline<T> _pipeline = new WorkPipeline<T>();
        private string _name;
        private TimeSpan _interval;
        private int _maxRetries;

        public WorkBuilder<T> Setup(string name, TimeSpan interval, int maxRetries = 3)
        {
            _name = name;
            _interval = interval;
            _maxRetries = maxRetries;
            return this;
        }

        public WorkBuilder<T> Use(Func<WorkContext<T>, Task<WorkResult>> runner)
        {
            _pipeline.Enqueue(provider => new WorkMiddleware<T>(runner));
            return this;
        }

        public WorkBuilder<T> Use<TMiddleware>() where TMiddleware : IWorkMiddleware<T>
        {
            _pipeline.Enqueue(provider => provider.GetRequiredService<TMiddleware>());
            return this;
        }

        internal Work<T> Build()
        {
            return new Work<T>(_pipeline)
            {
                JobName = _name,
                Interval = _interval,
                MaxRetries = _maxRetries,
            };
        }

        internal WorkBuilder() { }
    }
}