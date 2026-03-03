using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Interface for Generic Length Service.
    /// Defines business operations for Length comparison.
    /// </summary>
    public interface ILengthService
    {
        /// <summary>
        /// Checks exact equality between two Length measurements.
        /// </summary>
        bool AreEqual(Length firstMeasurement, Length secondMeasurement);

        /// <summary>
        /// Checks equality with tolerance.
        /// </summary>
        bool AreEqualWithTolerance(Length firstMeasurement,Length secondMeasurement,double tolerance);
    }
}