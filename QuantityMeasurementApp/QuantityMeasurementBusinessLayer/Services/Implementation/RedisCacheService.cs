using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Services.Interface;
using StackExchange.Redis;

namespace QuantityMeasurementBusinessLayer.Services.Implementation
{
    /// <summary>JSON values in Redis via <see cref="IConnectionMultiplexer"/>; keys prefixed with <c>qm:</c>.</summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _mux;
        private readonly ILogger<RedisCacheService> _logger;
        private const string KeyPrefix = "qm:";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public RedisCacheService(IConnectionMultiplexer mux, ILogger<RedisCacheService> logger)
        {
            _mux = mux;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var db = _mux.GetDatabase();
                var redisKey = KeyPrefix + key;
                var val = await db.StringGetAsync(redisKey).ConfigureAwait(false);
                if (!val.HasValue) return default;
                return JsonSerializer.Deserialize<T>(val.ToString()!, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis GET failed for {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var db = _mux.GetDatabase();
                var redisKey = KeyPrefix + key;
                var json = JsonSerializer.Serialize(value, JsonOptions);
                var expiry = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10);
                await db.StringSetAsync(redisKey, json, expiry).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis SET failed for {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var db = _mux.GetDatabase();
                await db.KeyDeleteAsync(KeyPrefix + key).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis REMOVE failed for {Key}", key);
            }
        }
    }
}
