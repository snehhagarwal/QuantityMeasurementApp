using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC9: Extension methods for WeightUnit.
    /// Provides conversion factors and conversion to/from the base unit (kilogram).
    /// </summary>
    public static class WeightUnitExtensions
    {
        private const double GramsPerKilogram = 1000.0;
        private const double KilogramsPerPound = 0.453592; // 1 lb ≈ 0.453592 kg

        /// <summary>
        /// Returns the conversion factor for this unit relative to the base unit (kilogram).
        /// Example:
        /// - KILOGRAM => 1.0
        /// - GRAM     => 0.001
        /// - POUND    => 0.453592
        /// </summary>
        public static double GetConversionFactor(this WeightUnit unit)
        {
            return unit switch
            {
                WeightUnit.KILOGRAM => 1.0,
                WeightUnit.GRAM => 1.0 / GramsPerKilogram,
                WeightUnit.POUND => KilogramsPerPound,
                WeightUnit.UNKNOWN => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid WeightUnit: {unit}")
            };
        }

        /// <summary>
        /// Converts a value in this unit to the base unit (kilogram).
        /// </summary>
        public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            return unit switch
            {
                WeightUnit.KILOGRAM => value,
                WeightUnit.GRAM => value / GramsPerKilogram,
                WeightUnit.POUND => value * KilogramsPerPound,
                WeightUnit.UNKNOWN => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid WeightUnit: {unit}")
            };
        }

        /// <summary>
        /// Converts a base-unit value (kilogram) to this unit.
        /// </summary>
        public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValueInKilogram)
        {
            if (!double.IsFinite(baseValueInKilogram))
                throw new ArgumentException("Base value must be a finite number");

            return unit switch
            {
                WeightUnit.KILOGRAM => baseValueInKilogram,
                WeightUnit.GRAM => baseValueInKilogram * GramsPerKilogram,
                WeightUnit.POUND => baseValueInKilogram / KilogramsPerPound,
                WeightUnit.UNKNOWN => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _ => throw new ArgumentException($"Invalid WeightUnit: {unit}")
            };
        }
    }
}