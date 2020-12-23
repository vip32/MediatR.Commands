namespace MediatR.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class MemoryCacheInvalidateCommandBehavior<TRequest, TResponse> : PipelineBehaviorBase<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCache cache;

        public MemoryCacheInvalidateCommandBehavior(ILoggerFactory loggerFactory, IMemoryCache cache)
            : base(loggerFactory)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override async Task<TResponse> Process(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // cache only if implements interface
            if (!(request is ICacheInvalidatedCommand instance))
            {
                return await next().ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(instance.CacheKey))
            {
                this.Logger.LogDebug("behavior: cache remove (key={cacheKey})", instance.CacheKey);
                this.cache.RemoveStartsWith(instance.CacheKey);
            }

            return await next().ConfigureAwait(false); // continue pipeline
        }
    }
}