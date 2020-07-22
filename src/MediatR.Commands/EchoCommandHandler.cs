namespace MediatR.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class EchoCommandHandler : CommandHandlerBase<CommandBase>
    {
        protected EchoCommandHandler(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<Unit> Process(CommandBase request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
