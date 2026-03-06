namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC14: Enum representing supported temperature units.
    /// Base unit for internal conversions is CELSIUS.
    ///
    /// Conversion formulas:
    ///   FAHRENHEIT → CELSIUS : (F - 32) × 5/9
    ///   CELSIUS    → FAHRENHEIT : (C × 9/5) + 32
    ///   KELVIN     → CELSIUS : K - 273.15
    ///   CELSIUS    → KELVIN  : C + 273.15
    /// </summary>
    public enum TemperatureUnit
    {
        UNKNOWN = 0,
        CELSIUS,
        FAHRENHEIT,
        KELVIN
    }
}