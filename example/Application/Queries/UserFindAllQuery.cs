namespace Application
{
    using System;
    using System.Collections.Generic;
    using MediatR.Commands;

    public class UserFindAllQuery : QueryBase<IEnumerable<User>>, ICachedQuery
    {
        public string CacheKey => nameof(UserFindAllQuery);

        public TimeSpan? SlidingExpiration => new TimeSpan(0, 0, 10);

        public DateTimeOffset? AbsoluteExpiration => null;
    }
}
