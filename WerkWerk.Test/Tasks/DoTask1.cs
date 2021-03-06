namespace WerkWerk.Test.Tasks
{
    using System.Threading.Tasks;
    using Model;

    public class DoTask1 : IWorkMiddleware<TestWorkerData>
    {
        public async Task<WorkResult> Execute(WorkContext<TestWorkerData> context)
        {
            if (context.Data.ForceFail)
            {
                return WorkResult.Fail(string.Empty);
            }

            await Task.Delay(50);
            context.Data.Task1Complete = true;
            return WorkResult.Success();
        }
    }
}