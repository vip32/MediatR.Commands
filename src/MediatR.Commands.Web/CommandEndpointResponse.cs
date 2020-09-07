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
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK,
            bool ignoreResponseBody = false)
        {
            this.OnSuccess = onSuccess;
            this.OnSuccessStatusCode = onSuccessStatusCode;
            this.IgnoreResponseBody = ignoreResponseBody;
        }

        public bool IgnoreResponseBody { get; }

        public HttpStatusCode OnSuccessStatusCode { get; }

        private Func<object, object, HttpContext, Task> OnSuccess { get; }

        public virtual void InvokeSuccess(object req, object res, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req, res, ctx);
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

        public override void InvokeSuccess(object req, object resp, HttpContext ctx)
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
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK,
            bool ignoreResponseBody = false)
            : base(null, onSuccessStatusCode, ignoreResponseBody)
        {
            this.OnSuccess = onSuccess;
        }

        private Func<TRequest, TResponse, HttpContext, Task> OnSuccess { get; }

        public override void InvokeSuccess(object req, object res, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req as TRequest, res as TResponse, ctx);
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
