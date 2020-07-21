namespace MediatR.Commands
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public abstract class QueryHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
        protected QueryHandlerBase(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        protected ILogger Logger { get; }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var type = request.GetType().Name;
            try
            {
                this.Logger.LogDebug("query: processing (type={queryType}, id={queryId})", type, request.QueryId);

                var timer = Stopwatch.StartNew();
                var response = await this.Process(request, cancellationToken).ConfigureAwait(false);
                timer.Stop();

                this.Logger.LogDebug("query: processed (type={queryType}, id={queryId}) -> took {elapsed} ms", type, request.QueryId, timer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "query: processing error (type={queryType}, id={queryId}): {errorMessage}", type, request.QueryId, ex.Message);
                throw;
            }
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken);
    }
}
