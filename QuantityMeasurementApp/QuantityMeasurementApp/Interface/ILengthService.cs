using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC3–UC7: Service interface for all generic Length operations.
    /// Covers equality, tolerance equality, conversion, and addition
    /// for all supported units (FEET, INCHES, YARDS, CENTIMETERS).
    /// Implemented by LengthService; registered in DI container for ASP.NET migration.
    /// </summary>
    public interface ILengthService
    {
        /// <summary>Returns true if both Length measurements represent the same physical length.</summary>
        bool AreEqual(Length first, Length second);

        /// <summary>Returns true if the two lengths are within the given tolerance (in inches).</summary>
        bool AreEqualWithTolerance(Length first, Length second, double toleranceInInches);

        /// <summary>UC5: Converts a Length to the specified target unit.</summary>
        Length ConvertTo(Length length, LengthUnit targetUnit, int decimalPlaces = 2);

        /// <summary>UC6: Adds two Length values and returns the result in the first operand's unit.</summary>
        Length Add(Length first, Length second, int decimalPlaces = 2);

        /// <summary>UC7: Adds two Length values and returns the result in the specified target unit.</summary>
        Length Add(Length first, Length second, LengthUnit targetUnit, int decimalPlaces = 2);
    }
}