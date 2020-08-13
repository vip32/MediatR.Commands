namespace MediatR.Commands
{
    using System;
    using System.Linq;

    public static class TypeExtensions
    {
        public static string PrettyName(this Type source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            if (source.IsGenericType)
            {
                const string genericOpen = "[";
                const string genericClose = "]";
                var name = source.Name.Substring(0, source.Name.IndexOf('`', StringComparison.OrdinalIgnoreCase));
                var types = string.Join(",", source.GetGenericArguments().Select(t => t.PrettyName()));
                return $"{name}{genericOpen}{types}{genericClose}";
            }

            return source.Name;
        }
    }
}
