namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Net.Http;

    public interface ICommandEndpointRegistrations
    {
        IEnumerable<CommandEndpointRegistrationItem> Items { get; }

        CommandEndpointRegistrationItem Add<TRequest>(string pattern, HttpMethod method);

        //CommandEndpointRegistrationQueryItem<TQuery> AddQuery<TQuery>(string pattern, HttpMethod method)
        //    where TQuery : IQuery;

        CommandEndpointRegistrationCommandItem<TCommand> AddCommand<TCommand>(string pattern, HttpMethod method)
            where TCommand : ICommand;
    }
}