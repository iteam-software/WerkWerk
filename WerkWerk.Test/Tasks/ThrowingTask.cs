using System.Threading.Tasks;

namespace WerkWerk.Test.Tasks
{
    using Model;
    public class ThrowingTask : IWorkMiddleware<TestWorkerData>
    {
        public Task<WorkResult> Execute(WorkContext<TestWorkerData> context)
        {
            throw new System.NotImplementedException();
        }
    }
}