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
}
