namespace MediatR.Commands
{
    using System;
    using MediatR;

    public interface ICommand : IRequest
    {
        string Id { get; }

        DateTimeOffset Timestamp { get; }
    }

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
        string Id { get; }

        DateTimeOffset Timestamp { get; }
    }
}
