namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public static partial class Extensions
    {
        /// <summary>
        /// Adds or updates the entry in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [DebuggerStepThrough]
        public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            TKey key,
            TValue value)
        {
            //source ??= new Dictionary<TKey, TValue>();

            if (source == null || key == null)
            {
                return source;
            }

            if (source.ContainsKey(key))
            {
                source.Remove(key);
            }

            source.Add(key, value);

            return source;
        }
    }
}