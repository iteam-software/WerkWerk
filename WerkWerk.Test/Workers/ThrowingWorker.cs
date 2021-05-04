using System;

namespace WerkWerk.Test.Workers
{
    using Model;
    using Tasks;

    public class ThrowingWorker : Worker<TestWorkerData>
    {
        private readonly TimeSpan _interval;

        public ThrowingWorker(IServiceProvider provider, TimeSpan interval) : base(provider)
        {
            _interval = interval;
        }

        protected override WorkBuilder<TestWorkerData> Configure(WorkBuilder<TestWorkerData> builder) => builder
            .Setup("ThrowingWork", _interval)
            .Use<ThrowingTask>();
    }
}