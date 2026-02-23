using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Interface defining business methods.
    /// </summary>
    public interface IFeetService
    {
        bool AreEqual(Feet firstMeasurement, Feet secondMeasurement);

        bool AreEqualWithTolerance(Feet firstMeasurement, Feet secondMeasurement, double tolerance);
    }
}