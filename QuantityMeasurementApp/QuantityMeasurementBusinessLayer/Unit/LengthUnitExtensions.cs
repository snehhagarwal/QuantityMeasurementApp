using System;
using QuantityMeasurementModel.Enums;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Unit
{
    /// <summary>
    /// UC5/UC8: Conversion logic for LengthUnit.
    /// Implements IUnitExtension — regular class, no static/extension methods.
    /// </summary>
    public class LengthUnitExtensions : IUnitExtension
    {
        private const double InchesPerFoot = 12.0;
        private const double FeetPerYard = 3.0;
        private const double InchesPerCentimeter = 0.393701;

        private readonly LengthUnit _unit;

        public LengthUnitExtensions(LengthUnit unit) => _unit = unit;

        public double GetConversionFactor() =>
            _unit switch
            {
                LengthUnit.FEET        => 1.0,
                LengthUnit.INCHES      => 1.0 / InchesPerFoot,
                LengthUnit.YARDS       => FeetPerYard,
                LengthUnit.CENTIMETERS => 1.0 / (InchesPerFoot / InchesPerCentimeter),
                LengthUnit.UNKNOWN     => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _                      => throw new ArgumentException($"Invalid LengthUnit: {_unit}")
            };

        public double ConvertToBaseUnit(double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            return _unit switch
            {
                LengthUnit.FEET        => value,
                LengthUnit.INCHES      => value / InchesPerFoot,
                LengthUnit.YARDS       => value * FeetPerYard,
                LengthUnit.CENTIMETERS => (value * InchesPerCentimeter) / InchesPerFoot,
                LengthUnit.UNKNOWN     => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                      => throw new ArgumentException($"Invalid LengthUnit: {_unit}")
            };
        }

        public double ConvertFromBaseUnit(double baseValueInFeet)
        {
            if (!double.IsFinite(baseValueInFeet))
                throw new ArgumentException("Base value must be a finite number");
            return _unit switch
            {
                LengthUnit.FEET        => baseValueInFeet,
                LengthUnit.INCHES      => baseValueInFeet * InchesPerFoot,
                LengthUnit.YARDS       => baseValueInFeet / FeetPerYard,
                LengthUnit.CENTIMETERS => (baseValueInFeet * InchesPerFoot) / InchesPerCentimeter,
                LengthUnit.UNKNOWN     => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                      => throw new ArgumentException($"Invalid LengthUnit: {_unit}")
            };
        }
    }
}
