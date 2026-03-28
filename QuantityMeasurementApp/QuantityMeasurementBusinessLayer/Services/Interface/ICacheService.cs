namespace QuantityMeasurementBusinessLayer.Services.Interface
{
    /// <summary>Distributed cache abstraction (Redis or in-memory fallback).</summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
