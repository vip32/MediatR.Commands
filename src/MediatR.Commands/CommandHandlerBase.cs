namespace MediatR.Commands
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public abstract class CommandHandlerBase<TRequest> : IRequestHandler<TRequest>
        where TRequest : ICommand
    {
        protected CommandHandlerBase(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        protected ILogger Logger { get; }

        public virtual async Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.LogDebug("command: processing '{request}'", request);
                var watch = Stopwatch.StartNew();

                var response = await this.Process(request, cancellationToken).ConfigureAwait(false);

                watch.Stop();
                this.Logger.LogDebug("command: processed '{requestName}' -> took {elapsed} ms", request, watch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "command: processing error '{requestName}': {errorMessage}", request, ex.Message);
                throw;
            }
        }

        protected abstract Task<Unit> Process(TRequest request, CancellationToken cancellationToken);
    }

#pragma warning disable SA1402 // File may only contain a single type
    public abstract class CommandHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
#pragma warning restore SA1402 // File may only contain a single type
        where TRequest : ICommand<TResponse>
    {
        protected CommandHandlerBase(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        protected ILogger Logger { get; }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.LogDebug("command: processing '{request}' (id={id})", request, request.Id);

                var timer = Stopwatch.StartNew();
                var response = await this.Process(request, cancellationToken).ConfigureAwait(false);
                timer.Stop();

                this.Logger.LogDebug("command: processed '{requestName}' (id={id}) -> took {elapsed} ms", request, request.Id, timer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "command: processing error '{requestName}' (id={id}): {errorMessage}", request, request.Id, ex.Message);
                throw;
            }
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken);
    }
}
