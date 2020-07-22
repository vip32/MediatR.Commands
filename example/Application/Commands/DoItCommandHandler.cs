namespace Application
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.Extensions.Logging;

    public class DoItCommandHandler : CommandHandlerBase<DoItCommand>
    {
        public DoItCommandHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<Unit> Process(DoItCommand request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}
