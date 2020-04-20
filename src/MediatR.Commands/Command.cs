namespace MediatR.Commands
{
    using System;

    public class Command : ICommand
    {
        public Command()
        {
            this.Id = Guid.NewGuid().ToString("N");
        }

        protected Command(string id)
        {
            this.Id = id;
        }

        public string Id { get; }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class Command<TResponse> : Command, ICommand<TResponse>
#pragma warning restore SA1402 // File may only contain a single type
    {
    }
}
