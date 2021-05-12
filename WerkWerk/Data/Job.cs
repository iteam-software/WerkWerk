
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public JobState Status { get; set; }
        public string Name { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int RetryCount { get; set; }
        public string RequestedBy { get; set; }
        public string Data { get; set; }
        public string Checksum { get; set; }

        public static implicit operator bool(Job job)
        {
            return job != null;
        }

        public static string GetData<T>(T data)
        {
            return JsonSerializer.Serialize(data);
        }

        public static string GetChecksum<T>(T data)
        {
            var str = GetData(data);
            return GetChecksum(str);
        }

        public static string GetChecksum(string str)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                var sha = new SHA256Managed();
                var bytes = sha.ComputeHash(stream);
                var checksum = new StringBuilder(bytes.Length * 2);

                foreach (var b in bytes)
                {
                    checksum.Append(b.ToString("x2"));
                }

                return checksum.ToString();
            }
        }

        public static void DefaultEntitySetup(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.Name, e.Checksum });
            builder.HasIndex(e => e.Checksum);
        }
    }
}