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
            var type = this.GetType().Name;
            var id = string.Empty;
            if (request is ICommand command)
            {
                id = command.CommandId;
                this.Logger.LogDebug("behavior: processing (type={behaviorType}, id={commandId})", type, id);
            }
            else if (request is IQuery query)
            {
                id = query.QueryId;
                this.Logger.LogDebug("behavior: processing (type={behaviorType}, id={queryId})", type, id);
            }

            try
            {
                var timer = Stopwatch.StartNew();
                var response = await this.Process(request, cancellationToken, next).ConfigureAwait(false);
                timer.Stop();

                if (request is ICommand)
                {
                    this.Logger.LogDebug("behavior: processed (type={behaviorType}, id={commandId}) -> took {elapsed} ms", type, id, timer.ElapsedMilliseconds);
                }
                else if (request is IQuery)
                {
                    this.Logger.LogDebug("behavior: processed (type={behaviorType}, id={queryId}) -> took {elapsed} ms", type, id, timer.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                if (request is ICommand)
                {
                    this.Logger.LogError(ex, "behavior: processing error (type={behaviorType}, id={commandId}): {errorMessage}", type, id, ex.Message);
                }
                else if (request is IQuery)
                {
                    this.Logger.LogError(ex, "behavior: processing error (type={behaviorType}, id={queryId}): {errorMessage}", type, id, ex.Message);
                }

                throw;
            }
        }

        protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
}