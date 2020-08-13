namespace Application
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.Extensions.Logging;

    public class UpdateUserCommandHandler : CommandHandlerBase<UpdateUserCommand>
    {
        public UpdateUserCommandHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<Unit> Process(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // request.UserId should be the value from the route > /users/123
            return Unit.Task;
        }
    }
}
