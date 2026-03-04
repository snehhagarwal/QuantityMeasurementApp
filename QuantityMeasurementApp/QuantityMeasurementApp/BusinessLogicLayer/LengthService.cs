using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// UC3–UC7: Business Logic Layer for all Length operations.
    /// Implements ILengthService; delegates data operations to LengthRepository.
    /// Covers equality (UC3/UC4), conversion (UC5), and addition (UC6/UC7).
    /// </summary>
    public class LengthService : ILengthService
    {
        private readonly LengthRepository _repository;

        public LengthService()
        {
            _repository = new LengthRepository();
        }

        /// <summary>ASP.NET-ready constructor — accepts repository via dependency injection.</summary>
        public LengthService(LengthRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public bool AreEqual(Length first, Length second)
        {
            return _repository.Compare(first, second);
        }

        /// <inheritdoc/>
        public bool AreEqualWithTolerance(Length first, Length second, double toleranceInInches)
        {
            return _repository.CompareWithTolerance(first, second, toleranceInInches);
        }

        /// <inheritdoc/>
        public Length ConvertTo(Length length, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            return _repository.ConvertTo(length, targetUnit, decimalPlaces);
        }

        /// <inheritdoc/>
        public Length Add(Length first, Length second, int decimalPlaces = 2)
        {
            return _repository.Add(first, second, decimalPlaces);
        }

        /// <inheritdoc/>
        public Length Add(Length first, Length second, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            return _repository.Add(first, second, targetUnit, decimalPlaces);
        }
    }
}