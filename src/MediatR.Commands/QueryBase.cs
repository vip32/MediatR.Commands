namespace MediatR.Commands
{
    using System;

    public abstract class QueryBase : IQuery
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
