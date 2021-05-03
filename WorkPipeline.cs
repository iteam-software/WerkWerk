using System;
using System.Collections.Generic;

namespace WerkWerk
{
    internal class WorkPipeline : Queue<Func<IServiceProvider, IWorkMiddleware>> { }
}