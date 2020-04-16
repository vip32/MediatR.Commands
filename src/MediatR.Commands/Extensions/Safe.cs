namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    public static partial class Extensions
    {
        /// <summary>
        /// Converts an null list to an empty list. Also clears out possible 'null' items
        /// Avoids null ref exceptions.
        /// </summary>
        /// <typeparam name="TSource">the source.</typeparam>
        /// <param name="source">the source collection.</param>
        /// <returns>collection of sources.</returns>
        [DebuggerStepThrough]
        public static ICollection<TSource> Safe<TSource>(this ICollection<TSource> source)
        {
            return (source ?? new Collection<TSource>()).Where(i => i != null).ToList();
        }
    }
}
