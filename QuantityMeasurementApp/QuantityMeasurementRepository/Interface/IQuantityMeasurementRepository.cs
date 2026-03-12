using System.Collections.Generic;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementRepository.Repository
{
    /// <summary>
    /// UC15: Repository interface following Interface Segregation Principle.
    /// Abstracts implementation details (in-memory cache, database, file-system).
    /// Provides a clean contract for managing QuantityMeasurementEntity records.
    ///
    /// Current implementation: QuantityMeasurementCacheRepository (Singleton, in-memory + disk).
    /// Future: Can swap for a database-backed implementation without changing service code.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        /// <summary>Saves a measurement entity to the repository.</summary>
        void Save(QuantityMeasurementEntity entity);

        /// <summary>Returns all stored measurement entities.</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();

        /// <summary>Clears all stored measurements.</summary>
        void Clear();
    }
}
