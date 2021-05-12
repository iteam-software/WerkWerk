
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

    public class Job : IDisposable
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
        public JsonDocument Data { get; set; }
        public string Checksum { get; set; }

        public static implicit operator bool(Job job)
        {
            return job != null;
        }

        public static JsonDocument GetData<T>(T data)
        {
            return JsonDocument.Parse(
                JsonSerializer.Serialize(data)
            );
        }

        public static string GetChecksum<T>(T data)
        {
            var doc = GetData(data);
            return GetChecksum(doc);
        }

        public static string GetChecksum(JsonDocument doc)
        {
            using var writeStream = new MemoryStream();
            using var writer = new Utf8JsonWriter(writeStream);

            doc.WriteTo(writer);
            writer.Flush();

            using var stream = new MemoryStream(writeStream.ToArray());

            var sha = new SHA256Managed();
            var bytes = sha.ComputeHash(stream);
            var checksum = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
            {
                checksum.Append(b.ToString("x2"));
            }

            return checksum.ToString();
        }

        public static string DataToJson(JsonDocument data)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            data.WriteTo(writer);
            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public string DataToJson() => DataToJson(Data);

        public static JsonDocument JsonToData(string json)
        {
            return JsonDocument.Parse(json);
        }

        public static void DefaultEntitySetup(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.Name, e.Checksum });
            builder.HasIndex(e => e.Checksum);
        }

        public void Dispose() => Data?.Dispose();
    }
}