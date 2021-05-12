using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WerkWerk.Test
{
    using Data;
    using Model;

    public class WorkRepositoryTests
    {
        [Fact]
        public async void CancelJob()
        {
            var provider = BuildServiceProvider(nameof(CancelJob));
            var repo = provider.GetRequiredService<IWorkRepository>();
            using var context = provider.GetRequiredService<TestContext>();
            var job = await repo.New<TestWorkerData>("Test", "Han Solo", new TestWorkerData());

            repo.CancelJobSync(job);

            var entity = await context.Jobs.AsNoTracking().FirstOrDefaultAsync();
            Assert.NotNull(entity.CancelledAt);
            Assert.Equal(JobState.Cancelled, entity.Status);
        }

        [Fact]
        public async void StartJob()
        {
            var provider = BuildServiceProvider(nameof(StartJob));
            var repo = provider.GetRequiredService<IWorkRepository>();
            using var context = provider.GetRequiredService<TestContext>();
            var job = await repo.New<TestWorkerData>("Test", "Han Solo", new TestWorkerData());

            await repo.StartJob(job);

            var entity = await context.Jobs.AsNoTracking().FirstOrDefaultAsync();
            Assert.NotNull(entity.StartedAt);
            Assert.Equal(JobState.InProgress, entity.Status);
        }

        [Fact]
        public async void GetMatchedJobs()
        {
            var provider = BuildServiceProvider(nameof(CancelJob));
            var repo = provider.GetRequiredService<IWorkRepository>();
            using var context = provider.GetRequiredService<TestContext>();
            var job = await repo.New<TestWorkerData>("Test", "Han Solo", new TestWorkerData());

            var checksum = Job.GetChecksum(new TestWorkerData());

            var matches = repo.GetMatchedJobs("Test", checksum).AsNoTracking();
            Assert.NotEmpty(matches);
        }

        private IServiceProvider BuildServiceProvider(string databaseName)
        {
            var services = new ServiceCollection();
            services.AddWerk<TestContext>();
            services.AddDbContext<TestContext>(options => options.UseInMemoryDatabase(databaseName));

            return services.BuildServiceProvider();
        }
    }
}