using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Services.Interface;

namespace QuantityMeasurementBusinessLayer.Services.Implementation
{
    /// <summary>In-process cache when Redis is disabled or unavailable.</summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            _cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow,
            CancellationToken cancellationToken = default)
        {
            var opts = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10)
            };
            _cache.Set(key, value, opts);
            _logger.LogDebug("Memory cache SET {Key}", key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
