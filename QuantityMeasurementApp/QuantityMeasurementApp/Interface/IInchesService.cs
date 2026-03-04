using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC2: Service interface for Inches equality operations.
    /// Implemented by InchesService; registered in DI container for ASP.NET migration.
    /// </summary>
    public interface IInchesService
    {
        /// <summary>Returns true if both Inches measurements are exactly equal.</summary>
        bool AreEqual(Inches firstMeasurement, Inches secondMeasurement);

        /// <summary>Returns true if the difference between the two Inches values is within tolerance.</summary>
        bool AreEqualWithTolerance(Inches firstMeasurement, Inches secondMeasurement, double tolerance);
    }
}