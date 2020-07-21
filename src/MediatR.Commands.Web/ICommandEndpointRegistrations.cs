namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Net.Http;

    public interface ICommandEndpointRegistrations
    {
        IEnumerable<CommandEndpointRegistrationItem> Items { get; }

        CommandEndpointRegistrationItem Add<TRequest>(string pattern, HttpMethod method);
    }
}