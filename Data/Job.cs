
using System;

namespace WerkWerk.Data
{
    public enum JobState
    {
        Pending,
        InProgress,
        Cancelled,
        Failed,
        Complete
    }

    public class Job
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime CancelledAt { get; set; }
        public int RetryCount { get; set; }
        public string RequestedBy { get; set; }
        public JobState Status { get; set; }
        public object Data { get; set; }

        public static implicit operator bool(Job job)
        {
            return job != null;
        }
    }
}