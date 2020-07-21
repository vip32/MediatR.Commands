namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Template;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.DependencyInjection;

    public static class EndpointRouteBuilderExtensions
    {
        public static void MapGet<TQuery>(this IEndpointRouteBuilder endpoints, string pattern)
            where TQuery : IQuery
        {
            pattern = pattern ?? throw new ArgumentNullException($"Query {typeof(TQuery)} cannot have a null or empty route pattern.");
            var mediator = endpoints.ServiceProvider.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator has not added to IServiceCollection. You can add it with services.AddMediatR(...);");
            var routeTable = endpoints.ServiceProvider.GetService<IRouteTable>()
                ?? throw new InvalidOperationException("IRouteTable has not added to IServiceCollection. You can add it with services.AddCommandEndpoints(...);");

            var routeItem = routeTable.AddRoute<TQuery>(pattern, HttpMethod.Get); // singletonb

            var builder = endpoints.MapGet(pattern, EndpointRouteDelegate);
            builder.WithDisplayName(typeof(TQuery).Name);
            builder.WithMetadata(routeItem);
        }

        private static async Task EndpointRouteDelegate(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var routeItem = endpoint.Metadata.GetMetadata<RouteItem>();

            object model;
            if (context.Request.ContentLength.GetValueOrDefault() != 0)
            {
                try
                {
                    model = await JsonSerializer.DeserializeAsync(context.Request.Body, routeItem.RequestType, null, context.RequestAborted);
                }
                catch (JsonException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
                catch (Exception exception) when (exception is FormatException || exception is OverflowException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }
            else
            {
                model = Activator.CreateInstance(routeItem.RequestType);
            }

            // map path and querystring values to created model
            var values = GetParameterValues(routeItem.Pattern, context.Request.Path, context.Request.QueryString.Value);
            // TODO: set values on model
            SetProperties(model, values);

            var mediator = context.RequestServices.GetService<IMediator>();
            var response = await mediator.Send(model, context.RequestAborted).ConfigureAwait(false);
            context.Response.Headers.Add("content-type", "application/json");

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                response,
                response?.GetType() ?? routeItem.ResponseType,
                null,
                context.RequestAborted).ConfigureAwait(false);

            await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
        }

        private static IDictionary<string, object> GetParameterValues(string pattern, string requestPath, string query = null)
        {
            // path parameter values
            var template = TemplateParser.Parse(pattern.SliceTill("?"));
            var matcher = new TemplateMatcher(template, GetDefaults(template));
            var values = new RouteValueDictionary();
            matcher.TryMatch(requestPath.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? requestPath : $"/{requestPath}", values);
            var result = EnsureParameterConstraints(template, values);

            // query parameter values
            if (!query.IsNullOrEmpty())
            {
                foreach (var queryItem in QueryHelpers.ParseQuery(query))
                {
                    result.AddOrUpdate(queryItem.Key, queryItem.Value);
                }
            }

            return result;
        }

        private static IDictionary<string, object> EnsureParameterConstraints(RouteTemplate template, RouteValueDictionary values)
        {
            var result = new Dictionary<string, object>();

            // fixup the possible parameter constraints (:int :bool :datetime :decimal :double :float :guid :long) https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.2#route-constraint-reference
            // after matching all values are of type string, regardless of parameter constraint
            foreach (var value in values)
            {
                var parameter = template.Parameters.FirstOrDefault(p => p.Name.Equals(value.Key, StringComparison.OrdinalIgnoreCase));
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

        private static RouteValueDictionary GetDefaults(RouteTemplate template)
        {
            var result = new RouteValueDictionary();

            if (template.Parameters != null)
            {
                foreach (var parameter in template.Parameters)
                {
                    if (parameter.DefaultValue != null)
                    {
                        result.Add(parameter.Name, parameter.DefaultValue);
                    }
                }
            }

            return result;
        }

        private static void SetProperties(object instance, IDictionary<string, object> propertyItems)
        {
            foreach (var propertyInfo in instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var propertyItem in propertyItems.Safe())
                {
                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    if (propertyItem.Key.SafeEquals(propertyInfo.Name) && propertyItem.Value != null && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(instance, propertyItem.Value.To(propertyType), null);
                    }
                }
            }
        }
    }
}
