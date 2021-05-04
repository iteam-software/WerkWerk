using System;
using System.Collections.Generic;

namespace WerkWerk
{
    internal class WorkPipeline<T> : Queue<Func<IServiceProvider, IWorkMiddleware<T>>> { }
}