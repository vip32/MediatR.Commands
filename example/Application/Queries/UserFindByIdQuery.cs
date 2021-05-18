namespace Application
{
    using System;
    using MediatR.Commands;

    public class UserFindByIdQuery : QueryBase<User>, ICachedQuery
    {
        public UserFindByIdQuery()
        {
        }

        public UserFindByIdQuery(string id)
        {
            this.Id = id;
        }

        public string Id { get; set; }

        public string CacheKey => $"{nameof(UserFindByIdQuery)}_{this.Id}";

        public TimeSpan? SlidingExpiration => new TimeSpan(0, 0, 10);

        public DateTimeOffset? AbsoluteExpiration => null;
    }
}
