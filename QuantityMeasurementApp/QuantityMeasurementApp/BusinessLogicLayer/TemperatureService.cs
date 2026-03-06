using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// UC14: Business Logic Layer for Temperature operations.
    /// Implements ITemperatureService; delegates data operations to TemperatureRepository.
    /// </summary>
    public class TemperatureService : ITemperatureService
    {
        private readonly TemperatureRepository _repository;

        public TemperatureService()
        {
            _repository = new TemperatureRepository();
        }

        /// <summary>ASP.NET-ready constructor — accepts repository via dependency injection.</summary>
        public TemperatureService(TemperatureRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public bool AreEqual(Quantity<TemperatureUnitMeasurable> first, Quantity<TemperatureUnitMeasurable> second)
        {
            return _repository.Compare(first, second);
        }

        /// <inheritdoc/>
        public Quantity<TemperatureUnitMeasurable> ConvertTo(Quantity<TemperatureUnitMeasurable> temperature,
                                                              TemperatureUnitMeasurable targetUnit,
                                                              int decimalPlaces = 2)
        {
            return _repository.ConvertTo(temperature, targetUnit, decimalPlaces);
        }
    }
}