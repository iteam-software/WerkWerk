using System;
using System.Collections.Generic;

namespace WerkWerk
{
    public class WorkPipeline<T> : Queue<Func<IServiceProvider, IWorkMiddleware<T>>> { }
}