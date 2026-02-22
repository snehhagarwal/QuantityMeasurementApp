using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// Business Logic Layer.
    /// Applies business rules and calls repository.
    /// </summary>
    public class FeetService : IFeetService
    {
        private readonly FeetRepository repository;

        /// <summary>
        /// Constructor initializes repository.
        /// </summary>
        public FeetService()
        {
            repository = new FeetRepository();
        }

        /// <summary>
        /// Checks exact equality.
        /// </summary>
        public bool AreEqual(Feet firstMeasurement, Feet secondMeasurement)
        {
            return repository.CompareFeet(firstMeasurement, secondMeasurement);
        }

        /// <summary>
        /// Checks equality with tolerance.
        /// Unique feature.
        /// </summary>
        public bool AreEqualWithTolerance(Feet firstMeasurement, Feet secondMeasurement, double tolerance)
        {
            return repository.CompareFeetWithTolerance(firstMeasurement, secondMeasurement, tolerance);
        }
    }
}