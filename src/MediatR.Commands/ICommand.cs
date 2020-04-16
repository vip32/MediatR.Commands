namespace MediatR.Commands
{
    using MediatR;

    public interface ICommand : IRequest
    {
        string Id { get; }
    }

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
        string Id { get; }
    }
}
