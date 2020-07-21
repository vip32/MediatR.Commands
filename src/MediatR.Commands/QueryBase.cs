namespace MediatR.Commands
{
    using System;

    public abstract class QueryBase : IQuery
    {
        protected QueryBase()
        {
            this.QueryId = Guid.NewGuid().ToString("N");
            this.QueryTimestamp = DateTime.UtcNow;
        }

        protected QueryBase(string id)
        {
            this.QueryId = id;
        }

        public string QueryId { get; }

        public DateTimeOffset QueryTimestamp { get; }
    }
}
