using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace HTLLBB.Services
{
    public class RedisConnection : IRedisConnection
    {
        RedisConfig _redisConfig { get; set; }

        public RedisConnection(IOptions<RedisConfig> redisConfig) => 
            _redisConfig = redisConfig.Value;

        public ConnectionMultiplexer GetInstance() => 
            ConnectionMultiplexer.Connect(_redisConfig.Address);
    }
}
