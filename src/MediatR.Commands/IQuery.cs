namespace MediatR.Commands
{
    using System;
    using MediatR;

    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
        string Id { get; }

        DateTimeOffset Timestamp { get; }
    }
}
