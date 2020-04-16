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
            try
            {
                this.Logger.LogDebug("query: processing '{request}'", request);

                var timer = Stopwatch.StartNew();
                var response = await this.Process(request, cancellationToken).ConfigureAwait(false);
                timer.Stop();

                this.Logger.LogDebug("query: processed '{requestName}' -> took {elapsed} ms", request, timer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "query: processing error '{requestName}': {errorMessage}", request, ex.Message);
                throw;
            }
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken);
    }
}
