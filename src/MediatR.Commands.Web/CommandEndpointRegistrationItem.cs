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

        public OpenApiDetails OpenApi { get; internal set; }
    }
}
