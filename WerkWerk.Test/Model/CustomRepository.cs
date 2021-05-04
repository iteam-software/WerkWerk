using System;
using System.Threading;
using System.Threading.Tasks;

namespace WerkWerk.Test.Model
{
    using System.Linq;
    using Data;

    public class CustomRepository : IWorkRepository
    {
        private readonly Job _job = new Job
        {
            Name = "CustomRepositoryJob",
            CreatedAt = DateTime.UtcNow,
            Status = JobState.Pending,
        };

        public void CancelJobSync(Job job)
        {
            throw new System.NotImplementedException();
        }

        public Task Completejob(Job job, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        public Task FailJob(Job job, CancellationToken token = default)
        {
            _job.RetryCount++;

            return Task.CompletedTask;
        }

        public IQueryable<Job> GetMatchedJobs(string name, string checksum)
        {
            throw new NotImplementedException();
        }

        public Task<Job> GetNextJob(string name, int maxRetries, CancellationToken token = default)
        {
            return Task.FromResult(_job);
        }

        public Task<Job> New<T>(string name, string requestedBy, T data, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }

        public Task StartJob(Job job, CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }
    }
}