//namespace MediatR.Commands
//{
//    using System;
//    using System.Diagnostics;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using Microsoft.Extensions.Logging;

//    public abstract class CommandHandlerBase<TRequest> : IRequestHandler<TRequest>
//        where TRequest : ICommand
//    {
//        protected CommandHandlerBase(ILoggerFactory loggerFactory)
//        {
//            this.Logger = loggerFactory.CreateLogger(this.GetType());
//        }

//        protected ILogger Logger { get; }

//        public virtual async Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
//        {
//            var type = request.GetType().Name;
//            try
//            {
//                this.Logger.LogDebug("command: processing (type={commandType}, id={commandId})", type, request.CommandId);

//                var timer = Stopwatch.StartNew();
//                var response = await this.Process(request, cancellationToken).ConfigureAwait(false);
//                timer.Stop();

//                this.Logger.LogDebug("command: processed (type={commandType}, id={commandId}) -> took {elapsed} ms", type, request.CommandId, timer.ElapsedMilliseconds);

//                return response;
//            }
//            catch (Exception ex)
//            {
//                this.Logger.LogError(ex, "command: processing error (type={commandType}, id={commandId}): {errorMessage}", type, request.CommandId, ex.Message);
//                throw;
//            }
//        }

//        protected abstract Task<Unit> Process(TRequest request, CancellationToken cancellationToken);
//    }
//}
