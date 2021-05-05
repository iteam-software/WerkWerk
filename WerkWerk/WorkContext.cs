using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace WerkWerk
{
    using Data;

    public class WorkContext<T>
    {
        private ILogger _logger;
        private IServiceProvider _services;
        private Dictionary<string, object> _items = new Dictionary<string, object>();

        public string RequestedBy { get; private set; }
        public T Data { get; private set; }
        public ILogger Logger => _logger;
        public IDictionary<string, object> Items => _items;

        public IServiceProvider Services => _services;

        internal static WorkContext<T> FromJob(Job job, ILogger logger, IServiceProvider services)
        {
            return new WorkContext<T>
            {
                _logger = logger,
                _services = services,
                RequestedBy = job.RequestedBy,
                Data = JsonSerializer.Deserialize<T>(job.Data),
            };
        }
    }
}