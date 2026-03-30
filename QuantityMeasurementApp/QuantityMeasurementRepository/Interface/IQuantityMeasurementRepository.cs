using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Interface
{
    public interface IQuantityMeasurementRepository
    {
        Task<QuantityMeasurement> SaveAsync(QuantityMeasurement entity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurement>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurement>> FindByOperationTypeAsync(string operationType, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurement>> FindByMeasurementTypeAsync(string measurementType, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurement>> FindByCreatedAtAfterAsync(DateTime after, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurement>> FindByIsErrorTrueAsync(CancellationToken cancellationToken = default);
        Task<long> CountByOperationTypeAndIsErrorFalseAsync(string operationType, CancellationToken cancellationToken = default);
        Task<long> CountAsync(CancellationToken cancellationToken = default);
    }
}
