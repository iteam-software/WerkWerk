using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WerkWerk.Samples
{
    public class EmailWorker : Worker
    {
        public EmailWorker(IServiceProvider provider) : base(provider)
        {
        }

        protected override WorkBuilder Configure(WorkBuilder builder) => builder
            .Setup("EmailSender", TimeSpan.FromSeconds(2))
            .Use<AuthorizedRequestor>()
            .Use<EmailSendingMiddleware>()
            .Use(ctx =>
            {
                ctx.Logger.LogInformation("Sent requested email!");
                return Task.FromResult(WorkResult.Success());
            });
    }

    public class EmailSendingMiddleware : IWorkMiddleware
    {
        private readonly ISendMail _sender;

        public EmailSendingMiddleware(ISendMail sender)
        {
            _sender = sender;
        }

        public async Task<WorkResult> Execute(WorkContext context)
        {
            var message = context.Data;

            try
            {
                await _sender.SendMessage(message);
                return WorkResult.Success();
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex, "Failed to send");
                return WorkResult.Fail(ex.Message);
            }
        }
    }

    public class AuthorizedRequestor : IWorkMiddleware
    {
        private string[] _authorizedSenders = new String[] {
            "Han Solo",
            "Luke Skywalker"
        };

        public Task<WorkResult> Execute(WorkContext context)
        {
            if (_authorizedSenders.Contains(context.RequestedBy))
            {
                return Task.FromResult(WorkResult.Success());
            }

            return Task.FromResult(WorkResult.Fail("Requestor is not authorized to send mail with this worker"));
        }
    }

    public interface ISendMail
    {
        Task SendMessage(object message);
    }
}