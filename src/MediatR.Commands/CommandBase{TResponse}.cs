namespace MediatR.Commands
{
    public abstract class CommandBase<TResponse> : CommandBase, ICommand<TResponse>
    {
        protected CommandBase()
            : base()
        {
        }

        protected CommandBase(string id)
            : base(id)
        {
        }
    }
}
