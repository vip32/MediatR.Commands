namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public class CommandEndpointConfiguration : ICommandEndpointConfiguration
    {
        private List<CommandEndpointRegistration> registrations;

        public IEnumerable<CommandEndpointRegistration> Registrations => this.registrations;

        public CommandEndpointRegistration AddRegistration<TRequest>(string pattern, HttpMethod method)
        {
            this.registrations ??= new List<CommandEndpointRegistration>();
            var registration = new CommandEndpointRegistration
            {
                Name = typeof(TRequest).Name,
                Pattern = pattern,
                Method = method,
                RequestType = typeof(TRequest),
                ResponseType = typeof(TRequest).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))?.GetGenericArguments()?[0]
            };
            this.registrations.Add(registration);

            return registration;
        }
    }
}
