namespace MediatR.Commands
{
    public interface ICacheInvalidatedCommand
    {
        string CacheKey { get; }
    }
}
