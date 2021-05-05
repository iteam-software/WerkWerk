using System;
using System.Threading.Tasks;

namespace WerkWerk.Test.Workers
{
    using Model;
    using Tasks;

    public class TestWorker : Worker<TestWorkerData>
    {
        private readonly TimeSpan _interval;

        public TestWorker(IServiceProvider provider, TimeSpan interval) : base(provider)
        {
            _interval = interval;
        }

        protected override WorkBuilder<TestWorkerData> Configure(WorkBuilder<TestWorkerData> builder) => builder
            .Setup("TestWork", _interval)
            .Use<DoTask1>()
            .Use(async ctx =>
            {
                await Task.Delay(50);
                ctx.Data.Task2Complete = true;
                return WorkResult.Success();
            });
    }
}