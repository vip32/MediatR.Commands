namespace MediatR.Commands
{
    using System;

    public interface IQuery
    {
        string Id { get; }

        DateTimeOffset Timestamp { get; }
    }
}
