namespace Application
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR.Commands;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class UserFindByIdQueryHandler : QueryHandlerBase<UserFindByIdQuery, User>
    {
        private readonly IMemoryCache cache;

        public UserFindByIdQueryHandler(ILoggerFactory loggerFactory, IMemoryCache cache)
            : base(loggerFactory)
        {
            this.cache = cache;
        }

        protected override Task<User> Process(UserFindByIdQuery request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (this.cache.TryGetValue($"users_{request.UserId}", out User user))
                {
                    this.Logger.LogInformation($"USER FOUND: {request.UserId}");
                    return user;
                }

                this.Logger.LogInformation($"USER NOT FOUND: {request.UserId}");
                return null; // TODO: this does not cause 404 not found response
            });
        }
    }
}
