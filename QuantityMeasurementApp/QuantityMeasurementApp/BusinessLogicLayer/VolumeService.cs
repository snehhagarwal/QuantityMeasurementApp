using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// UC11: Business Logic Layer for all Volume operations.
    /// Implements IVolumeService; delegates data operations to VolumeRepository.
    /// Covers equality, conversion, and addition for LITRE, MILLILITRE, and GALLON.
    /// </summary>
    public class VolumeService : IVolumeService
    {
        private readonly VolumeRepository _repository;

        public VolumeService()
        {
            _repository = new VolumeRepository();
        }

        /// <summary>ASP.NET-ready constructor — accepts repository via dependency injection.</summary>
        public VolumeService(VolumeRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public bool AreEqual(Quantity<VolumeUnitMeasurable> first, Quantity<VolumeUnitMeasurable> second)
        {
            return _repository.Compare(first, second);
        }

        /// <inheritdoc/>
        public Quantity<VolumeUnitMeasurable> ConvertTo(Quantity<VolumeUnitMeasurable> volume,
                                                         VolumeUnitMeasurable targetUnit,
                                                         int decimalPlaces = 2)
        {
            return _repository.ConvertTo(volume, targetUnit, decimalPlaces);
        }

        /// <inheritdoc/>
        public Quantity<VolumeUnitMeasurable> Add(Quantity<VolumeUnitMeasurable> first,
                                                   Quantity<VolumeUnitMeasurable> second,
                                                   int decimalPlaces = 2)
        {
            return _repository.Add(first, second, decimalPlaces);
        }

        /// <inheritdoc/>
        public Quantity<VolumeUnitMeasurable> Add(Quantity<VolumeUnitMeasurable> first,
                                                   Quantity<VolumeUnitMeasurable> second,
                                                   VolumeUnitMeasurable targetUnit,
                                                   int decimalPlaces = 2)
        {
            return _repository.Add(first, second, targetUnit, decimalPlaces);
        }
    }
}