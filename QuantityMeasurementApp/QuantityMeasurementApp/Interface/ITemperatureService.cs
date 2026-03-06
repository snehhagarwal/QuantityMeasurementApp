using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC14: Service interface for Temperature operations.
    /// Temperature supports only equality comparison and unit conversion.
    /// Arithmetic operations (Add, Subtract, Divide) are not included —
    /// they are physically meaningless for absolute temperatures.
    /// </summary>
    public interface ITemperatureService
    {
        /// <summary>Returns true if both temperature quantities represent the same temperature.</summary>
        bool AreEqual(Quantity<TemperatureUnitMeasurable> first, Quantity<TemperatureUnitMeasurable> second);

        /// <summary>Converts a temperature quantity to the specified target unit.</summary>
        Quantity<TemperatureUnitMeasurable> ConvertTo(Quantity<TemperatureUnitMeasurable> temperature,
                                                       TemperatureUnitMeasurable targetUnit,
                                                       int decimalPlaces = 2);
    }
}