using System;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Unit
{
    /// <summary>
    /// UC11: Conversion logic for VolumeUnit.
    /// Implements IUnitExtension — regular class, no static/extension methods.
    /// </summary>
    public class VolumeUnitExtensions : IUnitExtension
    {
        private const double MillilitresPerLitre = 1000.0;
        private const double LitresPerGallon     = 3.78541;

        private readonly VolumeUnit _unit;

        public VolumeUnitExtensions(VolumeUnit unit) => _unit = unit;

        public double GetConversionFactor() =>
            _unit switch
            {
                VolumeUnit.LITRE      => 1.0,
                VolumeUnit.MILLILITRE => 1.0 / MillilitresPerLitre,
                VolumeUnit.GALLON     => LitresPerGallon,
                VolumeUnit.UNKNOWN    => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _                     => throw new ArgumentException($"Invalid VolumeUnit: {_unit}")
            };

        public double ConvertToBaseUnit(double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            return _unit switch
            {
                VolumeUnit.LITRE      => value,
                VolumeUnit.MILLILITRE => value / MillilitresPerLitre,
                VolumeUnit.GALLON     => value * LitresPerGallon,
                VolumeUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                     => throw new ArgumentException($"Invalid VolumeUnit: {_unit}")
            };
        }

        public double ConvertFromBaseUnit(double baseValueInLitres)
        {
            if (!double.IsFinite(baseValueInLitres))
                throw new ArgumentException("Base value must be a finite number");
            return _unit switch
            {
                VolumeUnit.LITRE      => baseValueInLitres,
                VolumeUnit.MILLILITRE => baseValueInLitres * MillilitresPerLitre,
                VolumeUnit.GALLON     => baseValueInLitres / LitresPerGallon,
                VolumeUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                     => throw new ArgumentException($"Invalid VolumeUnit: {_unit}")
            };
        }
    }
}
