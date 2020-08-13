namespace Application
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR.Commands;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class UserFindAllQueryHandler : QueryHandlerBase<UserFindAllQuery, IEnumerable<User>>
    {
        private readonly IMemoryCache cache;

        public UserFindAllQueryHandler(ILoggerFactory loggerFactory, IMemoryCache cache)
            : base(loggerFactory)
        {
            this.cache = cache;
        }

        protected override Task<IEnumerable<User>> Process(UserFindAllQuery request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                var users = new List<User>();

                // workaround to get all the values from the cache https://stackoverflow.com/questions/45597057/how-to-retrieve-a-list-of-memory-cache-keys-in-asp-net-core
                if (field.GetValue(this.cache) is ICollection collection)
                {
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty("Key");
                        var cacheKey = methodInfo.GetValue(item);
                        if (this.cache.TryGetValue(cacheKey, out User user))
                        {
                            if (user != null)
                            {
                                this.Logger.LogInformation($"USER FOUND: {user.Id}");
                                users.Add(user);
                            }
                        }
                    }

                    return users;
                }

                return Enumerable.Empty<User>();
            });
        }
    }
}
