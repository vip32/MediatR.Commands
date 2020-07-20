namespace MediatR.Commands
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public abstract class CommandHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
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
                this.Logger.LogDebug("command: processing (type={commandType}, id={commandId})", request.GetType().Name, request.Id);

                var timer = Stopwatch.StartNew();
                var response = await this.Process(request, cancellationToken).ConfigureAwait(false);
                timer.Stop();

                this.Logger.LogDebug("command: processed (type={commandType}, id={commandId}) -> took {elapsed} ms", request.GetType().Name, request.Id, timer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "command: processing error (type={commandType}, id={commandId}): {errorMessage}", request.GetType().Name, request.Id, ex.Message);
                throw;
            }
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken);
    }
}
