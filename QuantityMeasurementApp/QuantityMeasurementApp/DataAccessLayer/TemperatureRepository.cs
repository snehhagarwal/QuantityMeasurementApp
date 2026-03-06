using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// UC14: Data Access Layer for Temperature operations.
    /// Handles equality and conversion for Quantity&lt;TemperatureUnitMeasurable&gt;.
    /// Delegates all domain logic to the Quantity&lt;TUnit&gt; entity — keeping the DAL thin.
    /// </summary>
    public class TemperatureRepository
    {
        /// <summary>Compares two temperature quantities for equality (base-unit comparison with epsilon).</summary>
        public bool Compare(Quantity<TemperatureUnitMeasurable> first, Quantity<TemperatureUnitMeasurable> second)
        {
            return first.Equals(second);
        }

        /// <summary>Converts a temperature quantity to the specified target unit.</summary>
        public Quantity<TemperatureUnitMeasurable> ConvertTo(Quantity<TemperatureUnitMeasurable> temperature,
                                                              TemperatureUnitMeasurable targetUnit,
                                                              int decimalPlaces = 2)
        {
            return temperature.ConvertTo(targetUnit, decimalPlaces);
        }
    }
}