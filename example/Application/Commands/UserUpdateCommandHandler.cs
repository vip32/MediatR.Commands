namespace Application
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MediatR.Commands;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class UserUpdateCommandHandler : CommandHandlerBase<UserUpdateCommand>
    {
        private readonly IMemoryCache cache;

        public UserUpdateCommandHandler(ILoggerFactory loggerFactory, IMemoryCache cache)
            : base(loggerFactory)
        {
            this.cache = cache;
        }

        protected override Task<Unit> Process(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            // request.UserId should be the value from the route > /users/123
            return Task.Run(() =>
            {
                if (this.cache.TryGetValue($"users_{request.UserId}", out User user))
                {
                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;

                    this.Logger.LogInformation($"USER UPDATED: {user.Id}");
                }

                // TODO: bad request if user not found

                return Unit.Task;
            });
        }
    }
}
