using System;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Unit
{
    /// <summary>
    /// UC14: Conversion logic for TemperatureUnit.
    /// Implements IUnitExtension — regular class, no static/extension methods.
    /// Base unit is CELSIUS.
    /// </summary>
    public class TemperatureUnitExtensions : IUnitExtension
    {
        private readonly TemperatureUnit _unit;

        public TemperatureUnitExtensions(TemperatureUnit unit) => _unit = unit;

        public double GetConversionFactor() =>
            _unit switch
            {
                TemperatureUnit.CELSIUS    => 1.0,
                TemperatureUnit.FAHRENHEIT => 1.0,
                TemperatureUnit.KELVIN     => 1.0,
                TemperatureUnit.UNKNOWN    => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _                          => throw new ArgumentException($"Invalid TemperatureUnit: {_unit}")
            };

        public double ConvertToBaseUnit(double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            return _unit switch
            {
                TemperatureUnit.CELSIUS    => value,
                TemperatureUnit.FAHRENHEIT => (value - 32.0) * 5.0 / 9.0,
                TemperatureUnit.KELVIN     => value - 273.15,
                TemperatureUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                          => throw new ArgumentException($"Invalid TemperatureUnit: {_unit}")
            };
        }

        public double ConvertFromBaseUnit(double baseCelsius)
        {
            if (!double.IsFinite(baseCelsius))
                throw new ArgumentException("Base value must be a finite number");
            return _unit switch
            {
                TemperatureUnit.CELSIUS    => baseCelsius,
                TemperatureUnit.FAHRENHEIT => (baseCelsius * 9.0 / 5.0) + 32.0,
                TemperatureUnit.KELVIN     => baseCelsius + 273.15,
                TemperatureUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                          => throw new ArgumentException($"Invalid TemperatureUnit: {_unit}")
            };
        }
    }
}
