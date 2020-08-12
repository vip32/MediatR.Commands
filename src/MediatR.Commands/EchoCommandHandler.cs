namespace MediatR.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class EchoCommandHandler : CommandHandlerBase<EchoCommand, Unit>
    {
        protected EchoCommandHandler(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<Unit> Process(EchoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
