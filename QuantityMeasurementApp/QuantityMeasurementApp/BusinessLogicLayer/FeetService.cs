using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// UC1: Business Logic Layer for Feet equality.
    /// Implements IFeetService; delegates data operations to FeetRepository.
    /// </summary>
    public class FeetService : IFeetService
    {
        private readonly FeetRepository _repository;

        public FeetService()
        {
            _repository = new FeetRepository();
        }

        /// <summary>ASP.NET-ready constructor — accepts repository via dependency injection.</summary>
        public FeetService(FeetRepository repository)
        {
            _repository = repository;
        }

        public bool AreEqual(Feet firstMeasurement, Feet secondMeasurement)
        {
            return _repository.CompareFeet(firstMeasurement, secondMeasurement);
        }

        public bool AreEqualWithTolerance(Feet firstMeasurement, Feet secondMeasurement, double tolerance)
        {
            return _repository.CompareFeetWithTolerance(firstMeasurement, secondMeasurement, tolerance);
        }
    }
}