﻿namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public static partial class Extensions
    {
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) // TODO: or SafeAny()?
        {
            return source == null || !source.Any();
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<TSource>(this ICollection<TSource> source) // TODO: or SafeAny()?
        {
            return source == null || !source.Any();
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this Stream source)
        {
            return source == null || source.Length == 0;
        }

        //public static bool IsNullOrEmpty<TSource>(this IReadOnlyCollection<TSource> source)
        //{
        //    return source == null || !source.Any();
        //}

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this Guid source)
        {
            return IsNullOrEmptyInternal(source);
        }

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this Guid? source)
        {
            return IsNullOrEmptyInternal(source);
        }

        private static bool IsNullOrEmptyInternal(this Guid? source)
        {
            if (source == null)
            {
                return true;
            }

            if (source == default || source == Guid.Empty)
            {
                return true;
            }

            if (string.IsNullOrEmpty(source.ToString()))
            {
                return true;
            }

            return false;
        }
    }
}
