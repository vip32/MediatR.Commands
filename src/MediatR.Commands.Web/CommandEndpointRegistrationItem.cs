namespace MediatR.Commands
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class CommandEndpointRegistrationItem
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public HttpMethod Method { get; set; }

        public Type RequestType { get; set; }

        public Type ResponseType { get; set; }

        public bool HasResponse => this.ResponseType != null;

        public string GroupName { get; set; }

        public HttpStatusCode OnSuccessStatusCode { get; set; }

        public Func<object, HttpContext, Task> OnSuccess { get; set; }

        public OpenApiDetails OpenApi { get; set; }

        public CommandEndpointResponse Response { get; set; }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class CommandEndpointResponse
    {
        public HttpStatusCode OnSuccessStatusCode { get; set; }

        public Func<object, object, HttpContext, Task> OnSuccess { get; set; }
    }

    public class CommandEndpointResponseQuery<TQuery, TResponse> : CommandEndpointResponse
        where TQuery : IQuery
    {
        public new Func<TQuery, TResponse, HttpContext, Task> OnSuccess { get; set; }
    }

    public class CommandEndpointResponseCommand<TCommand, TResponse> : CommandEndpointResponse
        where TCommand : ICommand
    {
        public new Func<TCommand, TResponse, HttpContext, Task> OnSuccess { get; set; }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////

    //public class CommandEndpointRegistrationQueryItem<TQuery> : CommandEndpointRegistrationItem
    //    where TQuery : IQuery
    //{
    //    public new Func<TQuery, HttpContext, Task> OnSuccess { get; set; }
    //}

    public class CommandEndpointRegistrationCommandItem<TCommand> : CommandEndpointRegistrationItem
        where TCommand : ICommand
    {
        public new Func<TCommand, HttpContext, Task> OnSuccess { get; set; }
    }
}
