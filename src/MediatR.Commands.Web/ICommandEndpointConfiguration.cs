namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Net.Http;

    public interface ICommandEndpointConfiguration
    {
        IEnumerable<CommandEndpointRegistration> Registrations { get; }

        CommandEndpointRegistration AddRegistration<TRequest>(string pattern, HttpMethod method);
    }
}