using StackExchange.Redis;
using System.Text.Json;

namespace GaVL.Application.Systems
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key);
        Task RemoveKeyAsync(string key);
        Task<bool> KeyExist(string key);

        Task<T?> GetValue<T>(string key);
        Task SetValue<T>(string key, T value, TimeSpan? expiry = null);

        //Dùng để Rate Limit
        Task IncrementValue(string key);
    }
    public class RedisService : IRedisService
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private IDatabase Database => _connectionMultiplexer.GetDatabase();
        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (expiry != null)
            {
                await Database.StringSetAsync(key, value, expiry.Value);
                return;
            }
            await Database.StringSetAsync(key, value);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            return await Database.StringGetAsync(key);
        }

        public async Task RemoveKeyAsync(string key)
        {
            await Database.KeyDeleteAsync(key);
        }

        public async Task<bool> KeyExist(string key)
        {
            return await Database.KeyExistsAsync(key);
        }

        public async Task<T?> GetValue<T>(string key)
        {
            var json = await Database.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return default!;
            }
            return JsonSerializer.Deserialize<T>(json.ToString(), _jsonOptions);
        }

        public async Task SetValue<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (expiry != null)
            {
                await Database.StringSetAsync(key, JsonSerializer.Serialize(value, _jsonOptions), expiry.Value);
                return;
            }
            await Database.StringSetAsync(key, JsonSerializer.Serialize(value, _jsonOptions));
        }

        public async Task IncrementValue(string key)
        {
            await Database.StringIncrementAsync(key);
        }
    }
}
