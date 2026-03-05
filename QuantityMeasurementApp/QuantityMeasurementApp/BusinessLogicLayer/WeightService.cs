using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// UC9: Business Logic Layer for all Weight operations.
    /// Implements IWeightService; delegates data operations to WeightRepository.
    /// Covers equality, conversion, and addition for KILOGRAM, GRAM, and POUND.
    /// </summary>
    public class WeightService : IWeightService
    {
        private readonly WeightRepository _repository;

        public WeightService()
        {
            _repository = new WeightRepository();
        }

        /// <summary>ASP.NET-ready constructor — accepts repository via dependency injection.</summary>
        public WeightService(WeightRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public bool AreEqual(Weight first, Weight second)
        {
            return _repository.Compare(first, second);
        }

        /// <inheritdoc/>
        public Weight ConvertTo(Weight weight, WeightUnit targetUnit, int decimalPlaces = 2)
        {
            return _repository.ConvertTo(weight, targetUnit, decimalPlaces);
        }

        /// <inheritdoc/>
        public Weight Add(Weight first, Weight second, int decimalPlaces = 2)
        {
            return _repository.Add(first, second, decimalPlaces);
        }

        /// <inheritdoc/>
        public Weight Add(Weight first, Weight second, WeightUnit targetUnit, int decimalPlaces = 2)
        {
            return _repository.Add(first, second, targetUnit, decimalPlaces);
        }
    }
}