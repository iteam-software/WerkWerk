using System;
using System.Threading.Tasks;

namespace WerkWerk
{
    public interface IWorkMiddleware<T>
    {
        Task<WorkResult> Execute(WorkContext<T> context);
    }

    public class WorkMiddleware<T> : IWorkMiddleware<T>
    {
        private readonly Func<WorkContext<T>, Task<WorkResult>> _handler;

        public WorkMiddleware(Func<WorkContext<T>, Task<WorkResult>> handler)
        {
            _handler = handler;
        }

        public Task<WorkResult> Execute(WorkContext<T> context) => _handler(context);
    }
}