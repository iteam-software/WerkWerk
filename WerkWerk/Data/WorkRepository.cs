using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
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
        void CancelJobSync(Job job);
    }

    public class WorkRepository : IWorkRepository
    {
        private readonly DbContext _context;

        public WorkRepository(DbContext context)
        {
            _context = context;
        }

        public void CancelJobSync(Job job)
        {
            job.Status = JobState.Cancelled;
            job.CancelledAt = DateTime.UtcNow;

            _context.Attach(job);
            _context.SaveChanges();
        }

        public async Task Completejob(Job job, CancellationToken token = default)
        {
            job.CompletedAt = DateTime.UtcNow;
            job.Status = JobState.Complete;

            await _context.SaveChangesAsync(token);
        }

        public async Task FailJob(Job job, CancellationToken token = default)
        {
            job.RetryCount++;
            job.Status = JobState.Failed;

            await _context.SaveChangesAsync(token);
        }

        public async Task<Job> GetNextJob(string name, int maxRetries, CancellationToken token = default)
        {
            var job = await _context.Set<Job>()
                .Where(job => job.Status == JobState.Failed && job.RetryCount < maxRetries)
                .OrderBy(job => job.CreatedAt)
                .FirstOrDefaultAsync(token);

            if (job)
            {
                return job;
            }

            // Look for non-failed work to do
            return await _context.Set<Job>()
                .Where(job => job.Status == JobState.Pending)
                .OrderBy(job => job.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Job> New<T>(string name, string requestedBy, T data, CancellationToken token = default)
        {
            var job = new Job
            {
                Name = name,
                RequestedBy = requestedBy,
                CreatedAt = DateTime.UtcNow,
                Status = JobState.Pending,
                Data = JsonSerializer.Serialize<T>(data)
            };

            _context.Add(job);
            await _context.SaveChangesAsync(token);

            return job;
        }

        public async Task StartJob(Job job, CancellationToken token = default)
        {
            job.Status = JobState.InProgress;
            _context.Update(job);

            await _context.SaveChangesAsync(token);
        }
    }
}