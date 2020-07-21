namespace MediatR.Commands
{
    using System;

    public interface IQuery
    {
        string QueryId { get; }

        DateTimeOffset QueryTimestamp { get; }
    }
}
