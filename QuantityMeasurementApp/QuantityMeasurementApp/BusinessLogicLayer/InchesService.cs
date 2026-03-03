using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// Business Logic Layer.
    /// Applies business rules and calls repository.
    /// </summary>
    public class InchesService : IInchesService
    {
        private readonly InchesRepository repository;

        /// <summary>
        /// Constructor initializes repository.
        /// </summary>
        public InchesService()
        {
            repository = new InchesRepository();
        }

        /// <summary>
        /// Checks exact equality.
        /// </summary>
        public bool AreEqual(Inches firstMeasurement,Inches secondMeasurement)
        {
            return repository.CompareInches(firstMeasurement,secondMeasurement);
        }

        /// <summary>
        /// Checks equality with tolerance.
        /// </summary>
        public bool AreEqualWithTolerance(Inches firstMeasurement,Inches secondMeasurement,double tolerance)
        {
            return repository.CompareWithTolerance(firstMeasurement,secondMeasurement,tolerance);
        }
    }
}