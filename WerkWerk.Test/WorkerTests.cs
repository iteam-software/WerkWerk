using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WerkWerk.Test
{
    using Workers;
    using Data;
    using Model;
    using Microsoft.EntityFrameworkCore;

    public sealed class WorkerTests : IDisposable
    {
        private readonly ServiceProvider _provider;

        public WorkerTests()
        {
            var services = new ServiceCollection();
            services.AddWerk<TestContext>();
            services.AddDbContext<TestContext>(options => options.UseInMemoryDatabase("TestData"));

            _provider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            var context = _provider.GetRequiredService<TestContext>();
            context.Jobs.RemoveRange(context.Jobs);
            context.SaveChanges();

            _provider.Dispose();
        }

        [Fact]
        public async void ExecuteTestWorker()
        {
            //Given
            using var worker = new TestWorker(_provider);
            var repo = _provider.GetRequiredService<IWorkRepository>();
            var source = new CancellationTokenSource();

            await repo.New<TestWorkerData>("TestWork", "xunit", new TestWorkerData());
            await repo.New<TestWorkerData>("TestWork", "xunit", new TestWorkerData());

            //When
            worker.StartAsync(source.Token).Fire();

            // Wait for the worker to clear the work queue
            await Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => source.Cancel());

            //Then there should be no more work.
            var job = await repo.GetNextJob("TestWork", 3);
            Assert.Null(job);
        }
    }
}