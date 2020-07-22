namespace MediatR.Commands
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class CommandEndpointResponse
    {
        public CommandEndpointResponse(
            Func<object, object, HttpContext, Task> onSuccess = null,
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK)
        {
            this.OnSuccess = onSuccess;
            this.OnSuccessStatusCode = onSuccessStatusCode;
        }

        public HttpStatusCode OnSuccessStatusCode { get; }

        private Func<object, object, HttpContext, Task> OnSuccess { get; }

        public virtual void Invoke(object req, object resp, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req, resp, ctx);
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class CommandEndpointResponse<TRequest> : CommandEndpointResponse
        where TRequest : class
    {
        public CommandEndpointResponse(
            Func<TRequest, HttpContext, Task> onSuccess = null,
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK)
            : base(null, onSuccessStatusCode)
        {
            this.OnSuccess = onSuccess;
        }

        private Func<TRequest, HttpContext, Task> OnSuccess { get; }

        public override void Invoke(object req, object resp, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req as TRequest, ctx);
        }
    }

    public class CommandEndpointResponse<TRequest, TResponse> : CommandEndpointResponse
        where TRequest : class
        where TResponse : class
    {
        public CommandEndpointResponse(
            Func<TRequest, TResponse, HttpContext, Task> onSuccess = null,
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK)
            : base(null, onSuccessStatusCode)
        {
            this.OnSuccess = onSuccess;
        }

        private Func<TRequest, TResponse, HttpContext, Task> OnSuccess { get; }

        public override void Invoke(object req, object resp, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req as TRequest, resp as TResponse, ctx);
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
