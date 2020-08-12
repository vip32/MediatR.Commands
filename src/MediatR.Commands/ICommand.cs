namespace MediatR.Commands
{
    using System;

    public interface ICommand
    {
        string CommandId { get; }

        DateTimeOffset CommandTimestamp { get; }
    }
}
