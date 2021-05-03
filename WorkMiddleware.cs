using System;
using System.Threading.Tasks;

namespace WerkWerk
{
    public interface IWorkMiddleware
    {
        Task<WorkResult> Execute(WorkContext context);
    }

    public class WorkMiddleware : IWorkMiddleware
    {
        private readonly Func<WorkContext, Task<WorkResult>> _handler;

        public WorkMiddleware(Func<WorkContext, Task<WorkResult>> handler)
        {
            _handler = handler;
        }

        public Task<WorkResult> Execute(WorkContext context) => _handler(context);
    }
}