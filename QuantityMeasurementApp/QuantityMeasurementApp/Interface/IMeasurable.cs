namespace QuantityMeasurementApp.Interface
{
    /// <summary>
    /// UC10: Interface that all measurement unit types must implement.
    /// Enables the generic Quantity<TUnit> class to work with any unit category.
    /// </summary>
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();
    }
}