namespace MediatR.Commands
{
    using System;
    using System.Net.Http;

    public class CommandEndpointRegistrationItem
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public HttpMethod Method { get; set; }

        public Type RequestType { get; set; }

        public Type ResponseType { get; set; }
    }
}
