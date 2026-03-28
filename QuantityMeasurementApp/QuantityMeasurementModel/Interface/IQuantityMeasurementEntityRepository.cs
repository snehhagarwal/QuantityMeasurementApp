using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementModel.Interface
{
    /// <summary>UC15 persistence contract for <see cref="QuantityMeasurementEntity"/> records (JSON / JDBC adapters).</summary>
    public interface IQuantityMeasurementEntityRepository
    {
        void Save(QuantityMeasurementEntity entity);
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
        void Clear();
        IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType);
        IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType);
        int GetTotalCount();
        string GetPoolStatistics() => $"Repository type: {GetType().Name} | Total records: {GetTotalCount()}";
        void ReleaseResources() { }
    }
}
