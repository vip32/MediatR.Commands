namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Template;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static partial class EndpointRouteBuilderExtensions
    {
        public static void MapGet<TQuery>(this IEndpointRouteBuilder endpoints, string pattern, CommandEndpointResponse response = null, OpenApiDetails openApi = null)
            where TQuery : IQuery
        {
            endpoints.Map<TQuery>(pattern, HttpMethod.Get, response, openApi);
        }

        //public static void MapGet<TQuery>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK, Func<TQuery, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
        //    where TQuery : IQuery
        //{
        //    endpoints.MapQuery(pattern, HttpMethod.Get, onSuccessStatusCode, onSuccess, openApi);
        //}

        //public static void MapGet<TQuery, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK, Func<TQuery, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
        //    where TQuery : IQuery, IQuery<TResponse>
        //{
        //    endpoints.MapQuery(pattern, HttpMethod.Get, onSuccessStatusCode, onSuccess, openApi);
        //}

        public static void MapPost<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.Accepted, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.MapCommand(pattern, HttpMethod.Post, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapPost<TCommand, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.Accepted, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand, ICommand<TResponse>
        {
            endpoints.MapCommand(pattern, HttpMethod.Post, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapPut<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.Accepted, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.MapCommand(pattern, HttpMethod.Put, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapPut<TCommand, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.Accepted, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand, ICommand<TResponse>
        {
            endpoints.MapCommand(pattern, HttpMethod.Put, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapDelete<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.NoContent, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.MapCommand(pattern, HttpMethod.Delete, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapDelete<TCommand, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.NoContent, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand, ICommand<TResponse>
        {
            endpoints.MapCommand(pattern, HttpMethod.Delete, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapPatch<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.Accepted, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.MapCommand(pattern, HttpMethod.Patch, onSuccessStatusCode, onSuccess, openApi);
        }

        public static void MapPatch<TCommand, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, HttpStatusCode onSuccessStatusCode = HttpStatusCode.Accepted, Func<TCommand, HttpContext, Task> onSuccess = null, OpenApiDetails openApi = null)
            where TCommand : ICommand, ICommand<TResponse>
        {
            endpoints.MapCommand(pattern, HttpMethod.Patch, onSuccessStatusCode, onSuccess, openApi);
        }

        private static void Map<TRequest>(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            HttpMethod method,
            CommandEndpointResponse response = null,
            OpenApiDetails openApi = null)
        {
            if (pattern.IsNullOrEmpty())
            {
                throw new ArgumentNullException($"{typeof(TRequest)} cannot be registered with a null or empty route pattern.");
            }

            var mediator = endpoints.ServiceProvider.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator has not been added to IServiceCollection. You can add it with services.AddMediatR(...);");
            var registrations = endpoints.ServiceProvider.GetService<ICommandEndpointRegistrations>() // =singleton
                ?? throw new InvalidOperationException("ICommandEndpointRegistrations has not been added to IServiceCollection. You can add it with services.AddCommandEndpoints(...);");
            var registrationItem = registrations.Add<TRequest>(pattern, method);
            registrationItem.OpenApi = openApi;
            registrationItem.Response = response;

            var builder = endpoints.MapGet(pattern, CommandEndpointRequestDelegate);
            builder.WithDisplayName(registrationItem.Name);
            builder.WithMetadata(registrationItem);
        }

        //private static void MapQuery<TQuery>(
        //    this IEndpointRouteBuilder endpoints,
        //    string pattern,
        //    HttpMethod method,
        //    HttpStatusCode onSuccessStatusCode,
        //    Func<TQuery, HttpContext, Task> onSuccess = null,
        //    OpenApiDetails openApi = null)
        //    where TQuery : IQuery
        //{
        //    if (pattern.IsNullOrEmpty())
        //    {
        //        throw new ArgumentNullException($"{typeof(TQuery)} cannot be registered with a null or empty route pattern.");
        //    }

        //    var mediator = endpoints.ServiceProvider.GetService<IMediator>()
        //        ?? throw new InvalidOperationException("IMediator has not been added to IServiceCollection. You can add it with services.AddMediatR(...);");
        //    var registrations = endpoints.ServiceProvider.GetService<ICommandEndpointRegistrations>() // =singleton
        //        ?? throw new InvalidOperationException("ICommandEndpointRegistrations has not been added to IServiceCollection. You can add it with services.AddCommandEndpoints(...);");
        //    var registrationItem = registrations.AddQuery<TQuery>(pattern, method);
        //    registrationItem.OnSuccessStatusCode = onSuccessStatusCode;
        //    registrationItem.OpenApi = openApi;
        //    registrationItem.OnSuccess = onSuccess;

        //    var builder = endpoints.MapGet(pattern, CommandEndpointRequestDelegate);
        //    builder.WithDisplayName(registrationItem.Name);
        //    builder.WithMetadata(registrationItem);
        //}

        private static void MapCommand<TCommand>(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            HttpMethod method,
            HttpStatusCode onSuccessStatusCode,
            Func<TCommand, HttpContext, Task> onSuccess = null,
            OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            if (pattern.IsNullOrEmpty())
            {
                throw new ArgumentNullException($"{typeof(TCommand)} cannot be registered with a null or empty route pattern.");
            }

            var mediator = endpoints.ServiceProvider.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator has not been added to IServiceCollection. You can add it with services.AddMediatR(...);");
            var registrations = endpoints.ServiceProvider.GetService<ICommandEndpointRegistrations>() // =singleton
                ?? throw new InvalidOperationException("ICommandEndpointRegistrations has not been added to IServiceCollection. You can add it with services.AddCommandEndpoints(...);");
            var registrationItem = registrations.AddCommand<TCommand>(pattern, method);
            registrationItem.OnSuccessStatusCode = onSuccessStatusCode;
            registrationItem.OpenApi = openApi;
            registrationItem.OnSuccess = onSuccess;

            var builder = endpoints.MapGet(pattern, CommandEndpointRequestDelegate);
            builder.WithDisplayName(registrationItem.Name);
            builder.WithMetadata(registrationItem); // must be last
        }

        private static async Task CommandEndpointRequestDelegate(HttpContext context)
        {
            var timer = Stopwatch.StartNew();
            var endpoint = context.GetEndpoint();
#pragma warning disable CA1826 // Do not use Enumerable methods on indexable collections. Instead use the collection directly
            var registrationItem = endpoint.Metadata.Last() as CommandEndpointRegistrationItem; //.GetMetadata<CommandEndpointRegistrationItem>();
#pragma warning restore CA1826 // Do not use Enumerable methods on indexable collections. Instead use the collection directly
            var logger = context.RequestServices.GetService<ILoggerFactory>().CreateLogger(registrationItem.RequestType);

            object requestModel;
            if (context.Request.ContentLength.GetValueOrDefault() != 0)
            {
                try
                {
                    requestModel = await JsonSerializer.DeserializeAsync(context.Request.Body, registrationItem.RequestType, null, context.RequestAborted);
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
                requestModel = Activator.CreateInstance(registrationItem.RequestType);
            }

            var id = string.Empty;
            if (requestModel is ICommand command)
            {
                id = command.CommandId;
            }
            else if (requestModel is IQuery query)
            {
                id = query.QueryId;
            }

            logger.LogDebug("request: starting (type={commandRequestType}, id={commandId}), method={commandRequestMethod}) {commandRequestUri}", registrationItem.RequestType.Name, id, context.Request.Method.ToUpper(), context.Request.GetUri().ToString());

            // map path and querystring values to created request model
            var parameterItems = GetParameterValues(registrationItem.Pattern, context.Request.Path, context.Request.QueryString.Value);
            ReflectionHelper.SetProperties(requestModel, parameterItems);

            var mediator = context.RequestServices.GetService<IMediator>();
            var response = await mediator.Send(requestModel, context.RequestAborted).ConfigureAwait(false);
            context.Response.Headers.Add("content-type", "application/json");

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                response,
                response.GetType() ?? registrationItem.ResponseType,
                null,
                context.RequestAborted).ConfigureAwait(false);

            await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
            timer.Stop();
            logger.LogDebug("request: finished (type={commandRequestType}, id={commandId})) -> took {elapsed} ms", registrationItem.RequestType.Name, id, timer.ElapsedMilliseconds);
        }

        private static IDictionary<string, object> GetParameterValues(string pattern, string requestPath, string query = null)
        {
            // path parameter values
            var template = TemplateParser.Parse(pattern.SliceTill("?"));
            var matcher = new TemplateMatcher(template, template.GetDefaults());
            var values = new RouteValueDictionary();
            matcher.TryMatch(requestPath.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? requestPath : $"/{requestPath}", values);
            var result = template.EnsureParameterConstraints(values);

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
    }
}
