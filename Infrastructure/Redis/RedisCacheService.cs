using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ShopNext.Infrastructure.Redis
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly StackExchange.Redis.IDatabase _db;
        private readonly RedisOptions _options;
        private readonly ILogger<RedisCacheService> _logger;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public RedisCacheService(
            IConnectionMultiplexer redis,
            IOptions<RedisOptions> options,
            ILogger<RedisCacheService> logger)
        {
            _db = redis.GetDatabase();
            _options = options.Value;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Redis GET failed for {Key}: {Message}", key, ex.Message);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                var expiry = ttl ?? TimeSpan.FromMinutes(_options.DefaultTtlMinutes);
                await _db.StringSetAsync(key, serialized, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Redis SET failed for {Key}: {Message}", key, ex.Message);
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Redis DELETE failed for {Key}: {Message}", key, ex.Message);
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl = null)
        {
            var cached = await GetAsync<T>(key);
            if (cached != null) return cached;

            var lockObj = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await lockObj.WaitAsync();
            try
            {
                cached = await GetAsync<T>(key);
                if (cached != null) return cached;

                var data = await factory();
                await SetAsync(key, data, ttl);
                return data;
            }
            finally
            {
                lockObj.Release();

                if (lockObj.CurrentCount == 1)
                    _locks.TryRemove(key, out _);
            }
        }
    }
}
