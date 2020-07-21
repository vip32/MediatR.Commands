namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public class CommandEndpointRegistrations : ICommandEndpointRegistrations
    {
        private List<CommandEndpointRegistrationItem> items;

        public IEnumerable<CommandEndpointRegistrationItem> Items => this.items;

        public CommandEndpointRegistrationItem Add<TRequest>(string pattern, HttpMethod method)
        {
            this.items ??= new List<CommandEndpointRegistrationItem>();

            var routeItem = new CommandEndpointRegistrationItem
            {
                Name = typeof(TRequest).Name,
                Pattern = pattern,
                Method = method,
                RequestType = typeof(TRequest),
                ResponseType = typeof(TRequest).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0]
            };

            this.items.Add(routeItem);

            return routeItem;
        }

        //public RouteItem GetRoute(string pattern, HttpMethod method)
        //{
        //    return this.routeItems.FirstOrDefault(r => r.Pattern)
        //}
    }
}
