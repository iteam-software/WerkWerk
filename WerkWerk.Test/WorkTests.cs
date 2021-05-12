using Xunit;

namespace WerkWerk.Test
{
    using Model;

    public class WorkTests
    {
        [Fact]
        public void WorkNull_implicit_bool()
        {
            var work = new Work<TestWorkerData>(
                new WorkPipeline<TestWorkerData>()
            );

            Assert.False((Work<TestWorkerData>)null);
            Assert.False(work);

            work.JobName = "Test";

            Assert.True(work);
        }
    }
}