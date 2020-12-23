namespace MediatR.Commands
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Caching.Memory;

    public static class MemoryCacheExtensions
    {
        private static readonly Func<MemoryCache, object> GetEntriesCollection = Delegate.CreateDelegate(
            typeof(Func<MemoryCache, object>),
            typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
            throwOnBindFailure: true) as Func<MemoryCache, object>;

        public static IEnumerable GetKeys(this IMemoryCache source) =>
            ((IDictionary)GetEntriesCollection((MemoryCache)source)).Keys;

        public static IEnumerable<T> GetKeys<T>(this IMemoryCache source) =>
            GetKeys(source).OfType<T>();

        public static void RemoveStartsWith(this IMemoryCache source, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            foreach (var foundKey in source.GetKeys<string>())
            {
                if (foundKey.StartsWith(key))
                {
                    source.Remove(foundKey);
                }
            }
        }

        public static void RemoveContains(this IMemoryCache source, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            foreach (var foundKey in source.GetKeys<string>())
            {
                if (foundKey.Contains(key))
                {
                    source.Remove(foundKey);
                }
            }
        }
    }
}