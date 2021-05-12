using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace WerkWerk
{
    using Data;

    public class WorkContext<T>
    {
        private CancellationToken _jobCancel;
        private ILogger _logger;
        private IServiceProvider _services;
        private Dictionary<string, object> _items = new Dictionary<string, object>();

        public string RequestedBy { get; private set; }
        public T Data { get; private set; }
        public ILogger Logger => _logger;
        public IDictionary<string, object> Items => _items;
        public CancellationToken JobCancel => _jobCancel;
        public IServiceProvider Services => _services;

        internal static WorkContext<T> FromJob(Job job, ILogger logger, IServiceProvider services, CancellationToken token = default)
        {
            var json = job.DataToJson();

            return new WorkContext<T>
            {
                _logger = logger,
                _services = services,
                _jobCancel = token,
                RequestedBy = job.RequestedBy,
                Data = JsonSerializer.Deserialize<T>(json),
            };
        }
    }
}