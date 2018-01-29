using StackExchange.Redis;

namespace HTLLBB.Services
{
    public interface IRedisConnection
    {
        ConnectionMultiplexer GetInstance();
    }
}
