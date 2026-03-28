using System;
using QuantityMeasurementModel.Enums;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Unit
{
    /// <summary>
    /// UC9: Conversion logic for WeightUnit.
    /// Implements IUnitExtension — regular class, no static/extension methods.
    /// </summary>
    public class WeightUnitExtensions : IUnitExtension
    {
        private const double GramsPerKilogram = 1000.0;
        private const double KilogramsPerPound = 0.453592;

        private readonly WeightUnit _unit;

        public WeightUnitExtensions(WeightUnit unit) => _unit = unit;

        public double GetConversionFactor() =>
            _unit switch
            {
                WeightUnit.KILOGRAM => 1.0,
                WeightUnit.GRAM     => 1.0 / GramsPerKilogram,
                WeightUnit.POUND    => KilogramsPerPound,
                WeightUnit.UNKNOWN  => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _                   => throw new ArgumentException($"Invalid WeightUnit: {_unit}")
            };

        public double ConvertToBaseUnit(double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            return _unit switch
            {
                WeightUnit.KILOGRAM => value,
                WeightUnit.GRAM     => value / GramsPerKilogram,
                WeightUnit.POUND    => value * KilogramsPerPound,
                WeightUnit.UNKNOWN  => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                   => throw new ArgumentException($"Invalid WeightUnit: {_unit}")
            };
        }

        public double ConvertFromBaseUnit(double baseValueInKilogram)
        {
            if (!double.IsFinite(baseValueInKilogram))
                throw new ArgumentException("Base value must be a finite number");
            return _unit switch
            {
                WeightUnit.KILOGRAM => baseValueInKilogram,
                WeightUnit.GRAM     => baseValueInKilogram * GramsPerKilogram,
                WeightUnit.POUND    => baseValueInKilogram / KilogramsPerPound,
                WeightUnit.UNKNOWN  => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                   => throw new ArgumentException($"Invalid WeightUnit: {_unit}")
            };
        }
    }
}
