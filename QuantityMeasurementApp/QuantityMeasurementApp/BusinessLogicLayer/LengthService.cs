using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.DataAccessLayer;
using QuantityMeasurementApp.Interfaces;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    /// <summary>
    /// Business logic for generic Length comparison.
    /// </summary>
    public class LengthService : ILengthService
    {
        private readonly LengthRepository repository;

        public LengthService()
        {
            repository = new LengthRepository();
        }

        /// <summary>
        /// Checks equality between two Length measurements.
        /// </summary>
        public bool AreEqual(Length first, Length second)
        {
            return repository.Compare(first, second);
        }

        public bool AreEqualWithTolerance(Length first,Length second,double tolerance)
        {
            return repository.CompareWithTolerance(first,second,tolerance);
        }
    }
}