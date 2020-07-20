namespace MediatR.Commands
{
    using System.Net.Http;

    public interface IRouteTable
    {
        RouteItem AddRoute<TRequest>(string pattern, HttpMethod method);
    }
}