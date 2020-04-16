namespace MediatR.Commands
{
    using System;

    public interface ICachedQuery
    {
        string CacheKey { get; }

        TimeSpan? SlidingExpiration { get; }

        DateTimeOffset? AbsoluteExpiration { get; }
    }
}