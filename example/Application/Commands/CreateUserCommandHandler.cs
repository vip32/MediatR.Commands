namespace Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.Extensions.Logging;

    public class CreateUserCommandHandler : CommandHandlerBase<CreateUserCommand, CreateUserCommandResponse>
    {
        public CreateUserCommandHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<CreateUserCommandResponse> Process(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return Task.Run(() => new CreateUserCommandResponse() { UserId = Guid.NewGuid().ToString() });
        }
    }
}
