using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WerkWerk.Test
{
    using Workers;
    using Data;
    using Model;
    using Tasks;

    public sealed class WorkerTests
    {

        [Fact]
        public async void ExecuteTestWorker()
        {
            //Given
            var provider = BuildServiceProvider(nameof(ExecuteTestWorker));
            using var worker = new TestWorker(provider, TimeSpan.FromMilliseconds(50));
            var repo = provider.GetRequiredService<IWorkRepository>();
            var source = new CancellationTokenSource();

            await repo.New<TestWorkerData>("TestWork", "xunit", new TestWorkerData());
            await repo.New<TestWorkerData>("TestWork", "xunit", new TestWorkerData());

            //When
            worker.StartAsync(source.Token).Fire();

            // Wait for the worker to clear the work queue
            await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => source.Cancel());

            //Then there should be no more work.
            var job = await repo.GetNextJob("TestWork", 3);
            Assert.Null(job);
        }

        [Fact]
        public async void ExecuteTestWorker_FAIL()
        {
            //Given
            var provider = BuildServiceProvider(nameof(ExecuteTestWorker_FAIL));
            using var worker = new TestWorker(provider, TimeSpan.FromMilliseconds(50));
            var context = provider.GetRequiredService<TestContext>();
            var repo = provider.GetRequiredService<IWorkRepository>();
            var source = new CancellationTokenSource();

            await repo.New<TestWorkerData>("TestWork", "xunit", new TestWorkerData { ForceFail = true });

            //When
            worker.StartAsync(source.Token).Fire();

            // Wait for the worker to clear the work queue
            await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => source.Cancel());

            var job = await context.Jobs.AsNoTracking().FirstOrDefaultAsync();
            Assert.Equal(JobState.Failed, job.Status);
        }

        [Fact]
        public async void ExecuteTestWorker_TaskThrows()
        {
            //Given
            var provider = BuildServiceProvider(nameof(ExecuteTestWorker_TaskThrows));
            using var worker = new ThrowingWorker(provider, TimeSpan.FromMilliseconds(50));
            var context = provider.GetRequiredService<TestContext>();
            var repo = provider.GetRequiredService<IWorkRepository>();
            var source = new CancellationTokenSource();

            await repo.New<TestWorkerData>("ThrowingWork", "xunit", new TestWorkerData { ForceFail = true });

            //When
            worker.StartAsync(source.Token).Fire();

            // Wait for the worker to clear the work queue
            await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => source.Cancel());

            var job = await context.Jobs.AsNoTracking().FirstOrDefaultAsync();
            Assert.Equal(JobState.Failed, job.Status);
            Assert.Equal(3, job.RetryCount);
        }

        [Fact]
        public async void ExecuteTestWorker_InvalidWorker()
        {
            //Given
            var provider = BuildServiceProvider(nameof(ExecuteTestWorker_InvalidWorker));
            using var worker = new InvalidWorker(provider);
            var context = provider.GetRequiredService<TestContext>();
            var repo = provider.GetRequiredService<IWorkRepository>();
            var source = new CancellationTokenSource();

            await repo.New<TestWorkerData>("InvalidWork", "xunit", new TestWorkerData { ForceFail = true });

            //When
            await worker.StartAsync(source.Token);

            var job = await context.Jobs.AsNoTracking().FirstOrDefaultAsync();
            Assert.Equal(JobState.Pending, job.Status);
        }

        [Fact]
        public async void ExecuteTestWorker_InvalidServiceProvider()
        {
            var provider = new ServiceCollection()
                .AddWerk<TestContext>()
                .AddDbContext<TestContext>(options => options.UseInMemoryDatabase(nameof(ExecuteTestWorker_InvalidServiceProvider)))
                .BuildServiceProvider();

            using var worker = new TestWorker(provider, TimeSpan.FromMilliseconds(50));
            var context = provider.GetRequiredService<TestContext>();
            var repo = provider.GetRequiredService<IWorkRepository>();
            var source = new CancellationTokenSource();

            var job = await repo.New<TestWorkerData>("TestWork", "xunit", new TestWorkerData { ForceFail = true });

            //When
            worker.StartAsync(source.Token).Fire();

            // Wait for the worker to clear the work queue
            await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => source.Cancel());

            job = await context.Jobs.AsNoTracking().FirstOrDefaultAsync();
            Assert.Equal(JobState.Failed, job.Status);
            Assert.Equal(3, job.RetryCount);
        }

        [Fact]
        public async void ExecuteTestWorker_CustomRepository_FAIL()
        {
            var provider = new ServiceCollection()
                .AddWerk<TestContext, CustomRepository>()
                .AddDbContext<TestContext>(options => options.UseInMemoryDatabase(nameof(ExecuteTestWorker_CustomRepository_FAIL)))
                .BuildServiceProvider();

            using var worker = new TestWorker(provider, TimeSpan.FromMilliseconds(50));
            var source = new CancellationTokenSource();

            //When
            worker.StartAsync(source.Token).Fire();

            // Wait for the worker to clear the work queue
            await Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => source.Cancel());
        }

        private IServiceProvider BuildServiceProvider(string databaseName)
        {
            var services = new ServiceCollection();
            services.AddWerk<TestContext>();
            services.AddDbContext<TestContext>(options => options.UseInMemoryDatabase(databaseName));
            services.AddScoped<DoTask1>();
            services.AddScoped<DoTask2>();
            services.AddScoped<ThrowingTask>();

            return services.BuildServiceProvider();
        }
    }
}