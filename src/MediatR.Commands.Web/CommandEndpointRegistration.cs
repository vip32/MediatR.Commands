namespace MediatR.Commands
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class CommandEndpointRegistration
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public HttpMethod Method { get; set; }

        public Type RequestType { get; set; }

        public Type ResponseType { get; set; }

        public bool HasResponse => this.ResponseType != null;

        public OpenApiDetails OpenApi { get; set; }

        public ICommandEndpointResponse Response { get; set; }
    }

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1201 // Elements should appear in the correct order
    public interface ICommandEndpointResponse
    {
        HttpStatusCode OnSuccessStatusCode { get; }

        //Func<object, object, HttpContext, Task> OnSuccess { get; set; }

        void Invoke(object req, object resp, HttpContext ctx);
    }

    //public interface ICommandEndpointResponse<TRequest, TResponse> : ICommandEndpointResponse
    //{
    //    new Func<TRequest, TResponse, HttpContext, Task> OnSuccess { get; set; }
    //}

    public class CommandEndpointResponse : ICommandEndpointResponse
    {
        public HttpStatusCode OnSuccessStatusCode { get; }

        private Func<object, object, HttpContext, Task> OnSuccess { get; }

        public CommandEndpointResponse(
            Func<object, object, HttpContext, Task> onSuccess = null,
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK)
        {
            this.OnSuccess = onSuccess;
            this.OnSuccessStatusCode = onSuccessStatusCode;
        }

        public virtual void Invoke(object req, object resp, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req, resp, ctx);
        }
    }

    public class CommandEndpointResponse<TRequest> : CommandEndpointResponse
        where TRequest : class
    {
        private Func<TRequest, HttpContext, Task> OnSuccess { get; }

        public CommandEndpointResponse(
            Func<TRequest, HttpContext, Task> onSuccess = null,
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK)
            : base(null, onSuccessStatusCode)
        {
            this.OnSuccess = onSuccess;
        }

        public override void Invoke(object req, object resp, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req as TRequest, ctx);
        }
    }

    public class CommandEndpointResponse<TRequest, TResponse> : CommandEndpointResponse
        where TRequest : class
        where TResponse : class
    {
        private Func<TRequest, TResponse, HttpContext, Task> OnSuccess { get; }

        public CommandEndpointResponse(
            Func<TRequest, TResponse, HttpContext, Task> onSuccess = null,
            HttpStatusCode onSuccessStatusCode = HttpStatusCode.OK)
            : base(null, onSuccessStatusCode)
        {
            this.OnSuccess = onSuccess;
        }

        public override void Invoke(object req, object resp, HttpContext ctx)
        {
            this.OnSuccess?.Invoke(req as TRequest, resp as TResponse, ctx);
        }
    }
}
