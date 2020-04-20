namespace MediatR.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class MemoryCacheQueryBehavior<TRequest, TResponse> : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache cache;

        public MemoryCacheQueryBehavior(ILoggerFactory loggerFactory, IMemoryCache memoryCache)
            : base(loggerFactory)
        {
            this.cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        protected override async Task<TResponse> Process(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // cache only if implements interface
            if (!(request is ICachedQuery query))
            {
                return await next().ConfigureAwait(false);
            }

            var cacheKey = query.CacheKey;
            if (this.cache.TryGetValue(cacheKey, out TResponse cachedResult))
            {
                this.Logger.LogDebug("cache hit; key: '{cacheKey}'.", cacheKey);
                return cachedResult;
            }

            this.Logger.LogDebug("cache miss; key: '{cacheKey}'.", cacheKey);

            var result = await next().ConfigureAwait(false); // continue if not found in cache
            if (result == null)
            {
                return default;
            }

            using (var entry = this.cache.CreateEntry(cacheKey))
            {
                entry.SlidingExpiration = query.SlidingExpiration;
                entry.AbsoluteExpiration = query.AbsoluteExpiration;
                entry.SetValue(result);

                this.Logger.LogDebug("cache set; key: '{cacheKey}'.", cacheKey);
            }

            return result;
        }
    }
}