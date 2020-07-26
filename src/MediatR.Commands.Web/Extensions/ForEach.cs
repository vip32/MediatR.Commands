namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public static partial class Extensions
    {
        /// <summary>
        /// Performs an action on each value of the enumerable.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="source">The items.</param>
        /// <param name="action">Action to perform on every item.</param>
        /// <returns>the source with the actions applied.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source.IsNullOrEmpty())
            {
                return source;
            }

            var itemsArray = source as T[] ?? source.ToArray();

            foreach (var value in itemsArray)
            {
                if (action != null && !EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    action(value);
                }
            }

            return itemsArray;
        }
    }
}
