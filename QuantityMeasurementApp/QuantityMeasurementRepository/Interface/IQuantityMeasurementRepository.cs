using System.Collections.Generic;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementRepository.Repository
{
    /// <summary>
    /// UC16: Enhanced repository interface with query, count, delete, and resource management.
    /// Abstracts both in-memory cache and database implementations.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        /// <summary>Saves a measurement entity to the repository.</summary>
        void Save(QuantityMeasurementEntity entity);

        /// <summary>Returns all stored measurement entities.</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();

        /// <summary>Clears all stored measurements.</summary>
        void Clear();

        /// <summary>UC16: Returns measurements filtered by operation type (e.g. "ADD", "COMPARE").</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType);

        /// <summary>UC16: Returns measurements filtered by measurement type (e.g. "LENGTH", "WEIGHT").</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType);

        /// <summary>UC16: Returns total count of stored measurements.</summary>
        int GetTotalCount();

        /// <summary>UC16: Default — returns pool/cache statistics string. Override for connection pools.</summary>
        string GetPoolStatistics() => $"Repository type: {GetType().Name} | Total records: {GetTotalCount()}";

        /// <summary>UC16: Default — releases any resources held (connections, file handles, etc.).</summary>
        void ReleaseResources() { }
    }
}
