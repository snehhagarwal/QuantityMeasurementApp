using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC5: Extension methods for LengthUnit enum.
    /// Provides conversion factor functionality as required by UC5.
    /// 
    /// This design pattern allows easy addition of new units by simply adding new enum constants
    /// with their conversion factors. Enums provide type safety and clarity.
    /// Enums can encapsulate related data and behavior, making the code more organized.
    /// The enum constants are also immutable by nature.
    /// </summary>
    public static class LengthUnitExtensions
    {
        /// <summary>
        /// UC5: Gets the conversion factor for a LengthUnit relative to the base unit (inches).
        /// 
        /// This method implements the "Enum with conversion factors" pattern required by UC5.
        /// The conversion factor represents how many inches are equivalent to 1 unit of the given LengthUnit.
        /// 
        /// Examples:
        /// - FEET.GetConversionFactor() returns 12.0 (1 foot = 12 inches)
        /// - INCHES.GetConversionFactor() returns 1.0 (base unit)
        /// - YARDS.GetConversionFactor() returns 36.0 (1 yard = 36 inches)
        /// - CENTIMETERS.GetConversionFactor() returns 0.393701 (1 cm = 0.393701 inches)
        /// 
        /// Immutability: Conversion factors remain constant throughout the application lifecycle.
        /// This makes enums thread-safe, as their state cannot be changed after creation.
        /// </summary>
        /// <param name="unit">The LengthUnit to get the conversion factor for</param>
        /// <returns>The conversion factor relative to the base unit (inches)</returns>
        /// <exception cref="ArgumentException">Thrown if the unit is UNKNOWN or invalid</exception>
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => 12.0,           // 1 foot = 12 inches
                LengthUnit.INCHES => 1.0,          // Base unit
                LengthUnit.YARDS => 36.0,          // 1 yard = 36 inches
                LengthUnit.CENTIMETERS => 0.393701, // 1 cm = 0.393701 inches
                LengthUnit.UNKNOWN => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid LengthUnit: {unit}")
            };
        }
    }
}
