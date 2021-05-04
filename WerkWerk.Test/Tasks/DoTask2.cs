namespace WerkWerk.Test.Tasks
{
    using System.Threading.Tasks;
    using Model;
    public class DoTask2 : IWorkMiddleware<TestWorkerData>
    {
        public async Task<WorkResult> Execute(WorkContext<TestWorkerData> context)
        {
            await Task.Delay(50);
            context.Data.Task2Complete = true;
            return WorkResult.Success();
        }
    }
}