using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC14: Extension methods for TemperatureUnit.
    /// Base unit is CELSIUS.
    ///
    /// Temperature conversions are non-linear (offset-based), unlike Length/Weight/Volume
    /// which use simple multiplication factors.
    ///
    /// Lambda expressions (Func&lt;double, double&gt;) encapsulate each conversion formula —
    /// mirrors the SupportsArithmetic functional interface pattern in IMeasurable.
    /// </summary>
    public static class TemperatureUnitExtensions
    {
        // Lambda expressions for each conversion path.
        // Celsius is the base unit — identity function.
        private static readonly Func<double, double> CelsiusToCelsius    = c => c;
        private static readonly Func<double, double> CelsiusToFahrenheit = c => (c * 9.0 / 5.0) + 32.0;
        private static readonly Func<double, double> CelsiusToKelvin     = c => c + 273.15;
        private static readonly Func<double, double> FahrenheitToCelsius = f => (f - 32.0) * 5.0 / 9.0;
        private static readonly Func<double, double> KelvinToCelsius     = k => k - 273.15;

        /// <summary>
        /// Temperature uses offset-based conversion, not a single multiplication factor.
        /// Returns 1.0 as a nominal placeholder; callers must use ConvertToBaseUnit / ConvertFromBaseUnit.
        /// </summary>
        public static double GetConversionFactor(this TemperatureUnit unit)
        {
            return unit switch
            {
                TemperatureUnit.CELSIUS    => 1.0,
                TemperatureUnit.FAHRENHEIT => 1.0,
                TemperatureUnit.KELVIN     => 1.0,
                TemperatureUnit.UNKNOWN    => throw new ArgumentException("Cannot get conversion factor for UNKNOWN unit"),
                _                          => throw new ArgumentException($"Invalid TemperatureUnit: {unit}")
            };
        }

        /// <summary>Converts a value in the given unit to the base unit (CELSIUS).</summary>
        public static double ConvertToBaseUnit(this TemperatureUnit unit, double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            return unit switch
            {
                TemperatureUnit.CELSIUS    => CelsiusToCelsius(value),
                TemperatureUnit.FAHRENHEIT => FahrenheitToCelsius(value),
                TemperatureUnit.KELVIN     => KelvinToCelsius(value),
                TemperatureUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                          => throw new ArgumentException($"Invalid TemperatureUnit: {unit}")
            };
        }

        /// <summary>Converts a base-unit value (CELSIUS) to the target unit.</summary>
        public static double ConvertFromBaseUnit(this TemperatureUnit unit, double baseCelsius)
        {
            if (!double.IsFinite(baseCelsius))
                throw new ArgumentException("Base value must be a finite number");

            return unit switch
            {
                TemperatureUnit.CELSIUS    => CelsiusToCelsius(baseCelsius),
                TemperatureUnit.FAHRENHEIT => CelsiusToFahrenheit(baseCelsius),
                TemperatureUnit.KELVIN     => CelsiusToKelvin(baseCelsius),
                TemperatureUnit.UNKNOWN    => throw new ArgumentException("Cannot convert UNKNOWN unit"),
                _                          => throw new ArgumentException($"Invalid TemperatureUnit: {unit}")
            };
        }
    }
}