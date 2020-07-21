namespace MediatR.Commands
{
    using System;
    using MediatR;

    public interface ICommand : IRequest
    {
        string CommandId { get; }

        DateTimeOffset CommandTimestamp { get; }
    }

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
        string CommandId { get; }

        DateTimeOffset CommandTimestamp { get; }
    }
}
