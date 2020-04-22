namespace MediatR.Commands
{
    using System;
    using MediatR;

    public abstract class QueryBase<TResponse> : IQuery<TResponse>
    {
        protected QueryBase()
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.Timestamp = DateTime.UtcNow;
        }

        protected QueryBase(string id)
        {
            this.Id = id;
        }

        public string Id { get; }

        public DateTimeOffset Timestamp { get; }
    }
}
