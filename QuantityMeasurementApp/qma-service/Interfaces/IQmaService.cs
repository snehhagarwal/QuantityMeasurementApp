using QmaService.Models;

namespace QmaService.Interfaces;

public interface IQmaService
{
    Task<QuantityMeasurementDto> CompareAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default);
    Task<QuantityMeasurementDto> ConvertAsync(QuantityOperandDto quantity, QuantityOperandDto target, long? userId, CancellationToken ct = default);
    Task<QuantityMeasurementDto> AddAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default);
    Task<QuantityMeasurementDto> SubtractAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default);
    Task<QuantityMeasurementDto> DivideAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default);
    Task<IEnumerable<QuantityMeasurementDto>> GetHistoryByUserAsync(long userId, CancellationToken ct = default);
    Task<IEnumerable<QuantityMeasurementDto>> GetHistoryByOperationTypeAndUserAsync(string operationType, long userId, CancellationToken ct = default);
    Task<IEnumerable<QuantityMeasurementDto>> GetHistoryByMeasurementTypeAsync(string measurementType, CancellationToken ct = default);
    Task<IEnumerable<QuantityMeasurementDto>> GetErrorHistoryByUserAsync(long userId, CancellationToken ct = default);
    Task<int> ClearHistoryByUserAsync(long userId, CancellationToken ct = default);
    Task<int> CountByOperationTypeAsync(string operationType, CancellationToken ct = default);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
