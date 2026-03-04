using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC11: Extension methods for VolumeUnit.
    /// Provides conversion factors and conversion to/from the base unit (litre).
    /// Mirrors the structure of LengthUnitExtensions and WeightUnitExtensions
    /// established in UC8 and UC9 respectively.
    /// </summary>
    public static class VolumeUnitExtensions
    {
        private const double MillilitresPerLitre = 1000.0;
        private const double LitresPerGallon     = 3.78541; // 1 US gallon ≈ 3.78541 L

        /// <summary>
        /// Returns the conversion factor for this unit relative to the base unit (litre).
        /// Example:
        ///   LITRE      => 1.0
        ///   MILLILITRE => 0.001
        ///   GALLON     => 3.78541
        /// </summary>
        public static double GetConversionFactor(this VolumeUnit unit)
        {
            return unit switch
            {
                VolumeUnit.LITRE      => 1.0,
                VolumeUnit.MILLILITRE => 1.0 / MillilitresPerLitre,
                VolumeUnit.GALLON     => LitresPerGallon,
                VolumeUnit.UNKNOWN    => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _                     => throw new ArgumentException($"Invalid VolumeUnit: {unit}")
            };
        }

        /// <summary>
        /// Converts a value in this unit to the base unit (litre).
        /// </summary>
        public static double ConvertToBaseUnit(this VolumeUnit unit, double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            return unit switch
            {
                VolumeUnit.LITRE      => value,
                VolumeUnit.MILLILITRE => value / MillilitresPerLitre,
                VolumeUnit.GALLON     => value * LitresPerGallon,
                VolumeUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                     => throw new ArgumentException($"Invalid VolumeUnit: {unit}")
            };
        }

        /// <summary>
        /// Converts a base-unit value (litre) to this unit.
        /// </summary>
        public static double ConvertFromBaseUnit(this VolumeUnit unit, double baseValueInLitres)
        {
            if (!double.IsFinite(baseValueInLitres))
                throw new ArgumentException("Base value must be a finite number");

            return unit switch
            {
                VolumeUnit.LITRE      => baseValueInLitres,
                VolumeUnit.MILLILITRE => baseValueInLitres * MillilitresPerLitre,
                VolumeUnit.GALLON     => baseValueInLitres / LitresPerGallon,
                VolumeUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                     => throw new ArgumentException($"Invalid VolumeUnit: {unit}")
            };
        }
    }
}