namespace MediatR.Commands
{
    using MediatR;

    public interface IQuery<out TResponse> : IQuery, IRequest<TResponse>
    {
    }
}
