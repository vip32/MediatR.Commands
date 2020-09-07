namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.Routing.Template;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class CommandEndpointRequestHandler
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            var logger = context.RequestServices.GetService<ILoggerFactory>().CreateLogger(typeof(CommandEndpointRequestHandler));
            var timer = Stopwatch.StartNew();
            var endpoint = context.GetEndpoint();
            var registration = endpoint.Metadata.GetMetadata<CommandEndpointRegistration>();
            var requestId = string.Empty;

            // prepare request model
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

            if (requestModel is ICommand command)
            {
                requestId = command.CommandId;
                logger.LogDebug("request: starting command (type={commandRequestType}, id={commandId}), method={commandRequestMethod}) {commandRequestUri}", registration.RequestType.Name, requestId, context.Request.Method.ToUpper(), context.Request.GetUri().ToString());
                context.Response.Headers.Add("X-CommandId", requestId);
            }
            else if (requestModel is IQuery query)
            {
                requestId = query.QueryId;
                logger.LogDebug("request: starting query (type={queryRequestType}, id={queryId}), method={queryRequestMethod}) {commandRequestUri}", registration.RequestType.Name, requestId, context.Request.Method.ToUpper(), context.Request.GetUri().ToString());
                context.Response.Headers.Add("X-QueryId", requestId);
            }
            else
            {
                // TODO: throw if unknown type
            }

            await SendRequest(context, registration, requestModel, requestId).ConfigureAwait(false);

            timer.Stop();
            if (requestModel is ICommand)
            {
                logger.LogDebug("request: finished command (type={commandRequestType}, id={commandId})) -> took {elapsed} ms", registration.RequestType.Name, requestId, timer.ElapsedMilliseconds);
            }
            else if (requestModel is IQuery)
            {
                logger.LogDebug("request: finished query (type={queryRequestType}, id={queryId})) -> took {elapsed} ms", registration.RequestType.Name, requestId, timer.ElapsedMilliseconds);
            }
        }

        private static async Task SendRequest(
            HttpContext context,
            CommandEndpointRegistration registration,
            object requestModel,
            string requestId)
        {
            var mediator = context.RequestServices.GetService<IMediator>();
            try
            {
                var response = await mediator.Send(requestModel, context.RequestAborted).ConfigureAwait(false);
                if (response is Unit) // Unit is the empty mediatr response
                {
                    response = null;
                }

                registration.Response?.InvokeSuccess(requestModel, response, context);
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
                }
            }
            catch (ValidationException ex) // 400
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Headers.Add("Content-Type", "application/problem+json");
                await JsonSerializer.SerializeAsync(
                        context.Response.Body,
                        new ProblemDetails
                        {
                            Status = (int)HttpStatusCode.BadRequest,
                            Title = "A validation error has occurred while executing the request",
                            Type = ex.GetType().Name,
                            Detail = ex.Message,
                            Instance = requestId
                        },
                        typeof(ProblemDetails),
                        null,
                        context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex) // 500
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers.Add("Content-Type", "application/problem+json");
                await JsonSerializer.SerializeAsync(
                        context.Response.Body,
                        new ProblemDetails
                        {
                            Status = (int)HttpStatusCode.InternalServerError,
                            Title = "An unhandled error has occurred while processing the request",
                            Type = ex.GetType().Name,
                            Detail = ex.Message,
                            Instance = requestId
                        },
                        typeof(ProblemDetails),
                        null,
                        context.RequestAborted).ConfigureAwait(false);
            }

            await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
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
