using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC1: Service interface for Feet equality operations.
    /// Implemented by FeetService; registered in DI container for ASP.NET migration.
    /// </summary>
    public interface IFeetService
    {
        /// <summary>Returns true if both Feet measurements are exactly equal.</summary>
        bool AreEqual(Feet firstMeasurement, Feet secondMeasurement);

        /// <summary>Returns true if the difference between the two Feet values is within tolerance.</summary>
        bool AreEqualWithTolerance(Feet firstMeasurement, Feet secondMeasurement, double tolerance);
    }
}