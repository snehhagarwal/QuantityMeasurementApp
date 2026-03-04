namespace QuantityMeasurementApp.Interface
{
    /// <summary>
    /// UC10: Contract that all measurement unit wrappers must implement.
    /// Enables the generic Quantity&lt;TUnit&gt; class to work with any unit category
    /// (Length, Weight, Volume, or any future category) without modification.
    /// </summary>
    public interface IMeasurable
    {
        /// <summary>Returns the conversion factor of this unit relative to its base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value in this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a base-unit value back to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns the display name of this unit.</summary>
        string GetUnitName();
    }
}