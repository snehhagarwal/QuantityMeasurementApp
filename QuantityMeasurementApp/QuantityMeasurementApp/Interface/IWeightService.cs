using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC9: Service interface for Weight operations.
    /// Covers equality, conversion, and addition
    /// for all supported weight units (KILOGRAM, GRAM, POUND).
    /// Implemented by WeightService; registered in DI container for ASP.NET migration.
    /// </summary>
    public interface IWeightService
    {
        /// <summary>Returns true if both Weight measurements represent the same physical weight.</summary>
        bool AreEqual(Weight first, Weight second);

        /// <summary>Converts a Weight to the specified target unit.</summary>
        Weight ConvertTo(Weight weight, WeightUnit targetUnit, int decimalPlaces = 2);

        /// <summary>Adds two Weight values and returns the result in the first operand's unit.</summary>
        Weight Add(Weight first, Weight second, int decimalPlaces = 2);

        /// <summary>Adds two Weight values and returns the result in the specified target unit.</summary>
        Weight Add(Weight first, Weight second, WeightUnit targetUnit, int decimalPlaces = 2);
    }
}