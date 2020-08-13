namespace Application
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR.Commands;
    using Microsoft.Extensions.Logging;

    public class UserByIdQueryHandler : QueryHandlerBase<UserByIdQuery, User>
    {
        public UserByIdQueryHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        protected override Task<User> Process(UserByIdQuery request, CancellationToken cancellationToken)
        {
            return Task.Run(() => new User { Id = request.UserId, FirstName = "firstname", LastName = "lastname" });
        }
    }
}
