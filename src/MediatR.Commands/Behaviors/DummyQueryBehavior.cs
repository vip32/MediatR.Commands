namespace MediatR.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DummyQueryBehavior<TRequest, TResponse> : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
        public DummyQueryBehavior(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override async Task<TResponse> Process(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            System.Console.Write("+++ DUMMY +++");
            this.Logger.LogDebug("query: DUMMY"); // BUG: not shown in console log

            return await next().ConfigureAwait(false); // continue pipeline
        }
    }
}