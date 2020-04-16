namespace MediatR.Commands
{
    using System;

    public interface ICacheQuery
    {
        string CacheKey { get; }

        TimeSpan? SlidingExpiration { get; }

        DateTimeOffset? AbsoluteExpiration { get; }
    }
}