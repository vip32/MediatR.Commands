namespace MediatR.Commands
{
    using System;
    using System.Net.Http;

    public class CommandEndpointRegistration
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public HttpMethod Method { get; set; }

        public Type RequestType { get; set; }

        public Type ResponseType { get; set; }

        public bool HasResponse => this.ResponseType != null;

        public OpenApiDetails OpenApi { get; set; }

        public CommandEndpointResponse Response { get; set; }
    }
}
