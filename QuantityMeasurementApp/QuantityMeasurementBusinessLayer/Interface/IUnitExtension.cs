namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// Contract for unit extension classes in the BusinessLayer.
    /// Each unit extension class (LengthUnitExtensions, WeightUnitExtensions,
    /// VolumeUnitExtensions, TemperatureUnitExtensions) provides these three operations
    /// for their respective unit enum.
    /// </summary>
    public interface IUnitExtension
    {
        /// <summary>Returns the conversion factor relative to the base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value from this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a value from the base unit to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);
    }
}
