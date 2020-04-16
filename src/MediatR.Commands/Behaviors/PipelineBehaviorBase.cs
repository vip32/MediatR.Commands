namespace MediatR.Commands
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public abstract class PipelineBehaviorBase<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        protected PipelineBehaviorBase(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        protected ILogger Logger { get; }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                this.Logger.LogDebug("Processing pipeline request '{request}' ...", request);
                var watch = Stopwatch.StartNew();

                var response = await this.Process(request, cancellationToken, next).ConfigureAwait(false);

                watch.Stop();
                this.Logger.LogDebug("Processed pipeline request '{request}' -> took {elapsed} ms", request, watch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error handling pipeline request '{request}': {errorMessage}", request, ex.Message);
                throw;
            }
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
}