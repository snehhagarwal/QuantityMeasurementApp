using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// UC2: Business Logic Layer for Inches equality.
    /// Implements IInchesService; delegates data operations to InchesRepository.
    /// </summary>
    public class InchesService : IInchesService
    {
        private readonly InchesRepository _repository;

        public InchesService()
        {
            _repository = new InchesRepository();
        }

        /// <summary>ASP.NET-ready constructor — accepts repository via dependency injection.</summary>
        public InchesService(InchesRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public bool AreEqual(Inches firstMeasurement, Inches secondMeasurement)
        {
            return _repository.CompareInches(firstMeasurement, secondMeasurement);
        }

        /// <inheritdoc/>
        public bool AreEqualWithTolerance(Inches firstMeasurement, Inches secondMeasurement, double tolerance)
        {
            return _repository.CompareWithTolerance(firstMeasurement, secondMeasurement, tolerance);
        }
    }
}