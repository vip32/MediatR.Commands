﻿namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            endpoints.Map<TQuery>(pattern, HttpMethod.Get, response ?? new CommandEndpointResponse(null, System.Net.HttpStatusCode.OK), openApi);
        }

        public static void MapPost<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, CommandEndpointResponse response = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.Map<TCommand>(pattern, HttpMethod.Post, response ?? new CommandEndpointResponse(null, System.Net.HttpStatusCode.OK), openApi);
        }

        public static void MapPut<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, CommandEndpointResponse response = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.Map<TCommand>(pattern, HttpMethod.Put, response ?? new CommandEndpointResponse(null, System.Net.HttpStatusCode.OK), openApi);
        }

        public static void MapDelete<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, CommandEndpointResponse response = null, OpenApiDetails openApi = null)
            where TCommand : ICommand
        {
            endpoints.Map<TCommand>(pattern, HttpMethod.Delete, response ?? new CommandEndpointResponse(null, System.Net.HttpStatusCode.OK), openApi);
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

            if (!pattern.StartsWith('/'))
            {
                pattern = $"/{pattern}"; // ensure leading pattern slash
            }

            var mediator = endpoints.ServiceProvider.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator has not been added to IServiceCollection. You can add it with services.AddMediatR(...);");
            var configuration = endpoints.ServiceProvider.GetService<ICommandEndpointConfiguration>() // =singleton
                ?? throw new InvalidOperationException("ICommandEndpointRegistrations has not been added to IServiceCollection. You can add it with services.AddCommandEndpoints(...);");
            var registration = configuration.AddRegistration<TRequest>(pattern, method);
            registration.OpenApi = openApi ?? new OpenApiDetails() { GroupName = pattern.SliceFromLast("/").SliceTill("?").SliceTill("{").EmptyToNull() ?? "Undefined" };
            registration.Response = response;

            IEndpointConventionBuilder builder = null;
            if (method == HttpMethod.Get)
            {
                builder = endpoints.MapGet(pattern.SliceTill("?"), CommandEndpointRequestDelegate);
            }
            else if (method == HttpMethod.Post)
            {
                builder = endpoints.MapPost(pattern.SliceTill("?"), CommandEndpointRequestDelegate);
            }
            else if (method == HttpMethod.Put)
            {
                builder = endpoints.MapPut(pattern.SliceTill("?"), CommandEndpointRequestDelegate);
            }
            else if (method == HttpMethod.Delete)
            {
                builder = endpoints.MapDelete(pattern.SliceTill("?"), CommandEndpointRequestDelegate);
            }

            //else if (method == HttpMethod.Patch)
            //{
            //    builder = endpoints.MapPatch(pattern.SliceTill("?"), CommandEndpointRequestDelegate);
            //}

            builder?.WithDisplayName(registration.Name);
            builder?.WithMetadata(registration);
        }

        private static async Task CommandEndpointRequestDelegate(HttpContext context)
        {
            var timer = Stopwatch.StartNew();
            var endpoint = context.GetEndpoint();
            var registration = endpoint.Metadata.GetMetadata<CommandEndpointRegistration>();
            // TODO: abort if no registration!
            var logger = context.RequestServices.GetService<ILoggerFactory>().CreateLogger(registration.RequestType);

            object requestModel;
            if (context.Request.ContentLength.GetValueOrDefault() != 0)
            {
                try
                {
                    // warning: json properties are case sensitive
                    requestModel = await JsonSerializer.DeserializeAsync(context.Request.Body, registration.RequestType, null, context.RequestAborted).ConfigureAwait(false);
                }
                catch (JsonException exception)
                {
                    logger.LogError($"deserialize: [{exception.GetType().Name}] {exception.Message}", exception);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
                catch (Exception exception) when (exception is FormatException || exception is OverflowException)
                {
                    logger.LogError($"deserialize: [{exception.GetType().Name}] {exception.Message}", exception);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }
            else
            {
                requestModel = Activator.CreateInstance(registration.RequestType);
            }

            // map path and querystring values to created request model
            var parameterItems = GetParameterValues(registration.Pattern, context.Request.Path, context.Request.QueryString.Value);
            ReflectionHelper.SetProperties(requestModel, parameterItems);

            var id = string.Empty;
            if (requestModel is ICommand command)
            {
                id = command.CommandId;
                logger.LogDebug("request: starting command (type={commandRequestType}, id={commandId}), method={commandRequestMethod}) {commandRequestUri}", registration.RequestType.Name, id, context.Request.Method.ToUpper(), context.Request.GetUri().ToString());
                context.Response.Headers.Add("X-CommandId", id);
            }
            else if (requestModel is IQuery query)
            {
                id = query.QueryId;
                logger.LogDebug("request: starting query (type={queryRequestType}, id={queryId}), method={queryRequestMethod}) {commandRequestUri}", registration.RequestType.Name, id, context.Request.Method.ToUpper(), context.Request.GetUri().ToString());
                context.Response.Headers.Add("X-QueryId", id);
            }
            else
            {
                // TODO: throw if unknown type
            }

            var mediator = context.RequestServices.GetService<IMediator>();
            try
            {
                var response = await mediator.Send(requestModel, context.RequestAborted).ConfigureAwait(false);
                if (response is Unit) // Unit is the empty mediatr response
                {
                    response = null;
                }

                registration.Response?.Invoke(requestModel, response, context);
                context.Response.StatusCode = (int)registration.Response.OnSuccessStatusCode;
                context.Response.Headers.Add("Content-Type", registration.OpenApi?.Produces);

                if (response != null && registration.Response?.IgnoreResponseBody == false)
                {
                    await JsonSerializer.SerializeAsync(
                        context.Response.Body,
                        response,
                        response?.GetType() ?? registration.ResponseType,
                        null,
                        context.RequestAborted).ConfigureAwait(false);
                    await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers.Add("Content-Type", "application/json");
                await JsonSerializer.SerializeAsync(
                        context.Response.Body,
                        new ProblemDetails
                        {
                            Status = (int)HttpStatusCode.InternalServerError,
                            Title = "An unhandled error occurred while processing the request",
                            Type = ex.GetType().Name,
                            Detail = ex.Message,
                            Instance = id
                        },
                        typeof(ProblemDetails),
                        null,
                        context.RequestAborted).ConfigureAwait(false);
                await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
            }

            timer.Stop();
            if (requestModel is ICommand)
            {
                logger.LogDebug("request: finished command (type={commandRequestType}, id={commandId})) -> took {elapsed} ms", registration.RequestType.Name, id, timer.ElapsedMilliseconds);
            }
            else if (requestModel is IQuery)
            {
                logger.LogDebug("request: finished query (type={queryRequestType}, id={queryId})) -> took {elapsed} ms", registration.RequestType.Name, id, timer.ElapsedMilliseconds);
            }
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