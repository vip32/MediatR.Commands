namespace MediatR.Commands
{
    public abstract class CommandBase<TResponse> : CommandBase, ICommand<TResponse>
    {
    }
}
