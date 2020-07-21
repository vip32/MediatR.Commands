namespace MediatR.Commands
{
    using System;

    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            this.CommandId = Guid.NewGuid().ToString("N");
            this.CommandTimestamp = DateTime.UtcNow;
        }

        protected CommandBase(string id)
        {
            this.CommandId = id;
        }

        public string CommandId { get; }

        public DateTimeOffset CommandTimestamp { get; }
    }
}
