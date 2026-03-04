using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC5/UC8: Extension methods for LengthUnit enum.
    ///
    /// UC5 introduced an enum-driven conversion approach (originally using INCHES as the base unit).
    /// UC8 refactors responsibility so the unit itself owns conversion to/from a base unit.
    ///
    /// In this project, we keep the UC5 conversion factors for backward-compatible behavior
    /// (notably CENTIMETERS uses 0.393701 inches per cm as in existing tests),
    /// while UC8-style conversion methods use FEET as the base unit.
    /// 
    /// This design pattern allows easy addition of new units by simply adding new enum constants
    /// with their conversion factors. Enums provide type safety and clarity.
    /// Enums can encapsulate related data and behavior, making the code more organized.
    /// The enum constants are also immutable by nature.
    /// </summary>
    public static class LengthUnitExtensions
    {
        // Canonical conversion constants (kept consistent with existing UC4/UC5 tests)
        private const double InchesPerFoot = 12.0;
        private const double FeetPerYard = 3.0;
        private const double InchesPerCentimeter = 0.393701; // matches existing tests

        /// <summary>
        /// UC8: Gets the conversion factor for a LengthUnit relative to the base unit (FEET).
        ///
        /// This represents how many FEET are equivalent to 1 unit of the given LengthUnit.
        ///
        /// Examples:
        /// - FEET.GetConversionFactor() returns 1.0  (1 foot = 1 foot)
        /// - INCHES.GetConversionFactor() returns 1/12 (~0.0833)
        /// - YARDS.GetConversionFactor() returns 3.0 (1 yard = 3 feet)
        /// - CENTIMETERS.GetConversionFactor() returns 1/30.48 (~0.0328)
        /// </summary>
        /// <param name="unit">The LengthUnit to get the conversion factor for</param>
        /// <returns>The conversion factor relative to the base unit (feet)</returns>
        /// <exception cref="ArgumentException">Thrown if the unit is UNKNOWN or invalid</exception>
        public static double GetConversionFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => 1.0,
                LengthUnit.INCHES => 1.0 / InchesPerFoot,
                LengthUnit.YARDS => FeetPerYard,
                LengthUnit.CENTIMETERS => 1.0 / (InchesPerFoot / InchesPerCentimeter), // 1 / 30.48
                LengthUnit.UNKNOWN => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid LengthUnit: {unit}")
            };
        }

        /// <summary>
        /// UC8: Converts a value in the given unit to the base unit FEET.
        /// </summary>
        public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            return unit switch
            {
                LengthUnit.FEET => value,
                LengthUnit.INCHES => value / InchesPerFoot,
                LengthUnit.YARDS => value * FeetPerYard,
                LengthUnit.CENTIMETERS => (value * InchesPerCentimeter) / InchesPerFoot,
                LengthUnit.UNKNOWN => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid LengthUnit: {unit}")
            };
        }

        /// <summary>
        /// UC8: Converts a base-unit value in FEET to the requested unit.
        /// </summary>
        public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValueInFeet)
        {
            if (!double.IsFinite(baseValueInFeet))
                throw new ArgumentException("Base value must be a finite number");

            return unit switch
            {
                LengthUnit.FEET => baseValueInFeet,
                LengthUnit.INCHES => baseValueInFeet * InchesPerFoot,
                LengthUnit.YARDS => baseValueInFeet / FeetPerYard,
                LengthUnit.CENTIMETERS => (baseValueInFeet * InchesPerFoot) / InchesPerCentimeter,
                LengthUnit.UNKNOWN => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid LengthUnit: {unit}")
            };
        }
    }
}