namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public class RouteTable : IRouteTable
    {
        private List<RouteItem> routeItems;

        public RouteItem AddRoute<TRequest>(string pattern, HttpMethod method)
        {
            this.routeItems ??= new List<RouteItem>();

            var routeItem = new RouteItem
            {
                Pattern = pattern,
                Method = method,
                RequestType = typeof(TRequest),
                ResponseType = typeof(TRequest).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0]
            };

            this.routeItems.Add(routeItem);

            return routeItem;
        }

        //public RouteItem GetRoute(string pattern, HttpMethod method)
        //{
        //    return this.routeItems.FirstOrDefault(r => r.Pattern)
        //}
    }
}
