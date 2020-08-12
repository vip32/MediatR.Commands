namespace MediatR.Commands
{
    using MediatR;

    public interface ICommand<out TResponse> : ICommand, IRequest<TResponse>
    {
    }
}
