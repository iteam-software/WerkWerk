using System;

namespace WerkWerk.Test.Workers
{
    using Model;
    public class InvalidWorker : Worker<TestWorkerData>
    {
        public InvalidWorker(IServiceProvider provider) : base(provider)
        {
        }

        protected override WorkBuilder<TestWorkerData> Configure(WorkBuilder<TestWorkerData> builder) => builder;
    }
}