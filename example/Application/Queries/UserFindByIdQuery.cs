namespace Application
{
    using System;
    using MediatR.Commands;

    public class UserFindByIdQuery : QueryBase<User>, ICachedQuery
    {
        public UserFindByIdQuery()
        {
        }

        public UserFindByIdQuery(string userId)
        {
            this.UserId = userId;
        }

        public string UserId { get; set; }

        public string CacheKey => nameof(UserFindByIdQuery);

        public TimeSpan? SlidingExpiration => new TimeSpan(0, 0, 10);

        public DateTimeOffset? AbsoluteExpiration => null;
    }
}
