using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Services.Interface
{
    public interface IQuantityMeasurementService
    {
        Task<QuantityMeasurementDto> CompareAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken cancellationToken = default);
        Task<QuantityMeasurementDto> ConvertAsync(QuantityOperandDto quantity, QuantityOperandDto target, CancellationToken cancellationToken = default);
        Task<QuantityMeasurementDto> AddAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken cancellationToken = default);
        Task<QuantityMeasurementDto> SubtractAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken cancellationToken = default);
        Task<QuantityMeasurementDto> DivideAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<QuantityMeasurementDto>> GetAllHistoryAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByOperationTypeAsync(string operationType, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByMeasurementTypeAsync(string measurementType, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<QuantityMeasurementDto>> GetErrorHistoryAsync(CancellationToken cancellationToken = default);
        Task<long> CountByOperationTypeAsync(string operationType, CancellationToken cancellationToken = default);
    }
}
