using System;
using Microsoft.Extensions.Logging;

namespace WerkWerk
{
    using Data;

    public class WorkContext
    {
        private ILogger _logger;
        private IServiceProvider _services;

        public string RequestedBy { get; private set; }
        public object Data { get; private set; }
        public ILogger Logger => _logger;

        public IServiceProvider Services => _services;

        internal static WorkContext FromJob(Job job, ILogger logger, IServiceProvider services)
        {
            return new WorkContext
            {
                _logger = logger,
                _services = services,
                RequestedBy = job.RequestedBy,
                Data = job.Data,
            };
        }
    }
}