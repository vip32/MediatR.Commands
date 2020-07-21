namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public class CommandEndpointRegistrations : ICommandEndpointRegistrations
    {
        private List<CommandEndpointRegistrationItem> items;

        public IEnumerable<CommandEndpointRegistrationItem> Items => this.items;

        public CommandEndpointRegistrationQueryItem<TQuery> AddQuery<TQuery>(string pattern, HttpMethod method)
           where TQuery : IQuery
        {
            this.items ??= new List<CommandEndpointRegistrationItem>();
            var item = new CommandEndpointRegistrationQueryItem<TQuery>
            {
                Name = typeof(TQuery).Name,
                Pattern = pattern,
                Method = method,
                RequestType = typeof(TQuery),
                ResponseType = typeof(TQuery).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))?.GetGenericArguments()?[0]
            };

            this.items.Add(item);

            return item;
        }

        public CommandEndpointRegistrationCommandItem<TCommand> AddCommand<TCommand>(string pattern, HttpMethod method)
            where TCommand : ICommand
        {
            this.items ??= new List<CommandEndpointRegistrationItem>();
            var item = new CommandEndpointRegistrationCommandItem<TCommand>
            {
                Name = typeof(TCommand).Name,
                Pattern = pattern,
                Method = method,
                RequestType = typeof(TCommand),
                ResponseType = typeof(TCommand).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))?.GetGenericArguments()[0]
            };

            this.items.Add(item);

            return item;
        }

        //public RouteItem GetRoute(string pattern, HttpMethod method)
        //{
        //    return this.routeItems.FirstOrDefault(r => r.Pattern)
        //}
    }
}
