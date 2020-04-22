namespace MediatR.Commands
{
    using System;

    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.Timestamp = DateTime.UtcNow;
        }

        protected CommandBase(string id)
        {
            this.Id = id;
        }

        public string Id { get; }

        public DateTimeOffset Timestamp { get; }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public abstract class CommandBase<TResponse> : CommandBase, ICommand<TResponse>
#pragma warning restore SA1402 // File may only contain a single type
    {
    }
}
