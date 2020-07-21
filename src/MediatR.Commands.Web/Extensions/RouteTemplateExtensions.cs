namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Template;

    public static class RouteTemplateExtensions
    {
        public static IDictionary<string, object> EnsureParameterConstraints(this RouteTemplate source, RouteValueDictionary values)
        {
            var result = new Dictionary<string, object>();

            // fixup the possible parameter constraints (:int :bool :datetime :decimal :double :float :guid :long) https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.2#route-constraint-reference
            // after matching all values are of type string, regardless of parameter constraint
            foreach (var value in values)
            {
                var parameter = source.Parameters.FirstOrDefault(p => p.Name.Equals(value.Key, StringComparison.OrdinalIgnoreCase));
                if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("int")) == true)
                {
                    result.Add(value.Key, value.Value.To<int>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("bool")) == true)
                {
                    result.Add(value.Key, value.Value.To<bool>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("datetime")) == true)
                {
                    result.Add(value.Key, value.Value.To<DateTime>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("decimal")) == true)
                {
                    result.Add(value.Key, value.Value.To<decimal>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("double")) == true)
                {
                    result.Add(value.Key, value.Value.To<double>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("float")) == true)
                {
                    result.Add(value.Key, value.Value.To<float>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("guid")) == true)
                {
                    result.Add(value.Key, value.Value.To<Guid>());
                }
                else if (parameter?.InlineConstraints?.Any(c => c.Constraint.SafeEquals("long")) == true)
                {
                    result.Add(value.Key, value.Value.To<long>());
                }
                else
                {
                    result.Add(value.Key, value.Value);
                }
            }

            return result;
        }

        public static RouteValueDictionary GetDefaults(this RouteTemplate source)
        {
            var result = new RouteValueDictionary();

            if (source.Parameters != null)
            {
                foreach (var parameter in source.Parameters)
                {
                    if (parameter.DefaultValue != null)
                    {
                        result.Add(parameter.Name, parameter.DefaultValue);
                    }
                }
            }

            return result;
        }
    }
}
