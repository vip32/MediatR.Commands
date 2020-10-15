namespace Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR.Commands;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class UserCreateCommandHandler : CommandHandlerBase<UserCreateCommand, UserCreateCommandResponse>
    {
        private readonly IMemoryCache cache;

        public UserCreateCommandHandler(ILoggerFactory loggerFactory, IMemoryCache cache)
            : base(loggerFactory)
        {
            this.cache = cache;
        }

        protected override Task<UserCreateCommandResponse> Process(UserCreateCommand request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString("N").Substring(29),
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                this.Logger.LogInformation($"USER CREATED: {user.Id}");
                this.cache.Set($"users_{user.Id}", user);
                return new UserCreateCommandResponse() { UserId = user.Id };
            });
        }
    }
}