namespace MediatR.Commands
{
    public abstract class QueryBase<TResponse> : QueryBase, IQuery<TResponse>
    {
        protected QueryBase()
            : base()
        {
        }

        protected QueryBase(string id)
            : base(id)
        {
        }
    }
}
