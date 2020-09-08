namespace MediatR.Commands
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class EndpointRouteBuilderExtensions
    {
        public static void MapGet<TQuery>(this IEndpointRouteBuilder endpoints, string pattern, string group = null, CommandEndpointResponse response = null, OpenApiOperation openApi = null)
            where TQuery : IQuery
        {
            endpoints.Map<TQuery>(
                pattern,
                HttpMethod.Get,
                group,
                response ?? new CommandEndpointResponse(null, HttpStatusCode.OK),
                openApi);
        }

        public static void MapPost<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, string group = null, CommandEndpointResponse response = null, OpenApiOperation openApi = null)
            where TCommand : ICommand
        {
            endpoints.Map<TCommand>(
                pattern,
                HttpMethod.Post,
                group,
                response ?? new CommandEndpointResponse(null, HttpStatusCode.OK),
                openApi);
        }

        public static void MapPut<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, string group = null, CommandEndpointResponse response = null, OpenApiOperation openApi = null)
            where TCommand : ICommand
        {
            endpoints.Map<TCommand>(
                pattern,
                HttpMethod.Put,
                group,
                response ?? new CommandEndpointResponse(null, HttpStatusCode.OK),
                openApi);
        }

        public static void MapDelete<TCommand>(this IEndpointRouteBuilder endpoints, string pattern, string group = null, CommandEndpointResponse response = null, OpenApiOperation openApi = null)
            where TCommand : ICommand
        {
            endpoints.Map<TCommand>(
                pattern,
                HttpMethod.Delete,
                group,
                response ?? new CommandEndpointResponse(null, HttpStatusCode.OK),
                openApi);
        }

        private static void Map<TRequest>(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            HttpMethod method,
            string group = null,
            CommandEndpointResponse response = null,
            OpenApiOperation openApi = null)
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
            registration.OpenApi = openApi ?? new OpenApiOperation() { GroupName = group ?? pattern.SliceFromLast("/").SliceTill("?").SliceTill("{").EmptyToNull() ?? "Undefined" };
            registration.Response = response;

            IEndpointConventionBuilder builder = null;
            if (method == HttpMethod.Get)
            {
                builder = endpoints.MapGet(pattern.SliceTill("?"), CommandEndpointRequestHandler.InvokeAsync);
            }
            else if (method == HttpMethod.Post)
            {
                builder = endpoints.MapPost(pattern.SliceTill("?"), CommandEndpointRequestHandler.InvokeAsync);
            }
            else if (method == HttpMethod.Put)
            {
                builder = endpoints.MapPut(pattern.SliceTill("?"), CommandEndpointRequestHandler.InvokeAsync);
            }
            else if (method == HttpMethod.Delete)
            {
                builder = endpoints.MapDelete(pattern.SliceTill("?"), CommandEndpointRequestHandler.InvokeAsync);
            }

            //else if (method == HttpMethod.Patch)
            //{
            //    builder = endpoints.MapPatch(pattern.SliceTill("?"), CommandEndpointRequestDelegate);
            //}

            builder?.WithDisplayName(registration.Name);
            builder?.WithMetadata(registration);
        }
    }
}