using System;
using Xunit;

namespace WerkWerk.Test
{
    using Data;

    public class JobTests
    {
        [Fact]
        public void GetChecksum_Data()
        {
            var data = new { foo = "bar" };

            var checksum = Job.GetChecksum<dynamic>(data);

            Assert.False(string.IsNullOrEmpty(checksum));
            Assert.Equal(
                Job.GetChecksum<dynamic>(new { foo = "bar" }),
                checksum
            );
        }

        [Fact]
        public void JobConstruction()
        {
            var job = new Job();

            Assert.NotEqual(default(Guid), job.Id);
            Assert.NotEqual(default(DateTime), job.CreatedAt);
            Assert.Equal(JobState.Pending, job.Status);
            Assert.Null(job.CancelledAt);
            Assert.Null(job.CompletedAt);
        }
    }
}