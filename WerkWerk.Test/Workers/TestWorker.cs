using System;

namespace WerkWerk.Test.Workers
{
    using Model;
    using Tasks;

    public class TestWorker : Worker<TestWorkerData>
    {
        public TestWorker(IServiceProvider provider) : base(provider)
        {
        }

        protected override WorkBuilder<TestWorkerData> Configure(WorkBuilder<TestWorkerData> builder) => builder
            .Setup("TestWork", TimeSpan.FromSeconds(1))
            .Use<DoTask1>()
            .Use<DoTask2>();
    }
}