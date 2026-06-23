using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ShopNext.Infrastructure.Redis
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisOptions _options;
        private readonly ILogger<RedisCacheService> _logger;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public RedisCacheService(
            IConnectionMultiplexer redis,
            IOptions<RedisOptions> options,
            ILogger<RedisCacheService> logger)
        {
            _redis = redis;
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
                _logger.LogWarning(ex, "Redis GET failed for {Key}", key);
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
                _logger.LogWarning(ex, "Redis SET failed for {Key}", key);
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
                _logger.LogWarning(ex, "Redis DELETE failed for {Key}", key);
            }
        }

        public async Task DeleteByPatternAsync(string pattern)
        {
            try
            {
                var endpoint = _redis.GetEndPoints().FirstOrDefault();

                if (endpoint == null)
                    return;

                var server = _redis.GetServer(endpoint);
                var keys = new List<RedisKey>();

                await foreach (var key in server.KeysAsync(pattern: pattern, pageSize: 250))
                {
                    keys.Add(key);
                }

                if (keys.Count > 0)
                {
                    await _db.KeyDeleteAsync(keys.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis pattern delete failed for {Pattern}", pattern);
            }
        }
        //otp cach
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis EXISTS failed for {Key}", key);
                return false;
            }
        }

        public async Task<long> IncrementAsync(string key, TimeSpan? expiry = null)
        {
            try
            {
                var value = await _db.StringIncrementAsync(key);

                if (value == 1 && expiry.HasValue)
                    await _db.KeyExpireAsync(key, expiry);

                return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis INCREMENT failed for {Key}", key);
                return 0;
            }
        }

        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? ttl = null)
        {
            var cached = await GetAsync<T>(key);

            if (cached is not null)
                return cached;

            var lockObj = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await lockObj.WaitAsync();

            try
            {
                cached = await GetAsync<T>(key);

                if (cached is not null)
                    return cached;

                var data = await factory();

                if (data is not null)
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