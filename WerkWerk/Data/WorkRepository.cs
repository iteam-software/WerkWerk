using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WerkWerk.Data
{
    public interface IWorkRepository
    {
        Task<Job> GetNextJob(string name, int maxRetries, CancellationToken token = default);
        Task<Job> New<T>(string name, string requestedBy, T data, CancellationToken token = default);
        Task StartJob(Job job, CancellationToken token = default);
        Task FailJob(Job job, CancellationToken token = default);
        Task Completejob(Job job, CancellationToken token = default);
        IQueryable<Job> GetMatchedJobs(string name, string checksum);
        void CancelJobSync(Job job);
    }

    public class WorkRepository<TContext> : IWorkRepository where TContext : DbContext
    {
        private readonly TContext _context;

        public WorkRepository(TContext context)
        {
            _context = context;
        }

        public void CancelJobSync(Job job)
        {
            job.Status = JobState.Cancelled;
            job.CancelledAt = DateTime.UtcNow;

            _context.Update(job);
            _context.SaveChanges();
        }

        public async Task Completejob(Job job, CancellationToken token = default)
        {
            job.CompletedAt = DateTime.UtcNow;
            job.Status = JobState.Complete;

            _context.Update(job);
            await _context.SaveChangesAsync(token);
        }

        public async Task FailJob(Job job, CancellationToken token = default)
        {
            job.RetryCount++;
            job.Status = JobState.Failed;

            _context.Update(job);
            await _context.SaveChangesAsync(token);
        }

        public IQueryable<Job> GetMatchedJobs(string name, string checksum)
        {
            return _context.Set<Job>().Where(job => job.Name == name && job.Checksum == checksum);
        }

        public async Task<Job> GetNextJob(string name, int maxRetries, CancellationToken token = default)
        {
            var job = await _context.Set<Job>()
                .Where(job => job.Name == name && job.Status == JobState.Failed && job.RetryCount < maxRetries)
                .OrderBy(job => job.CreatedAt)
                .FirstOrDefaultAsync(token);

            if (job)
            {
                return job;
            }

            // Look for non-failed work to do
            return await _context.Set<Job>()
                .Where(job => job.Name == name && job.Status == JobState.Pending)
                .OrderBy(job => job.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Job> New<T>(string name, string requestedBy, T data, CancellationToken token = default)
        {
            var str = Job.GetData(data);
            var job = new Job
            {
                Name = name,
                RequestedBy = requestedBy,
                Data = str,
                Checksum = Job.GetChecksum(str),
            };

            _context.Add(job);
            await _context.SaveChangesAsync(token);

            return job;
        }

        public async Task StartJob(Job job, CancellationToken token = default)
        {
            job.StartedAt = DateTime.UtcNow;
            job.Status = JobState.InProgress;
            _context.Update(job);
            await _context.SaveChangesAsync(token);
        }
    }
}