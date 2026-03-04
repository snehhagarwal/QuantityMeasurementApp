using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: IMeasurable wrapper for WeightUnit.
    /// Same pattern as LengthUnitMeasurable for the Weight category.
    /// </summary>
    public readonly struct WeightUnitMeasurable : IMeasurable
    {
        public WeightUnit Unit { get; }

        public WeightUnitMeasurable(WeightUnit unit)
        {
            Unit = unit;
        }

        public double GetConversionFactor() => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double baseValue) => Unit.ConvertFromBaseUnit(baseValue);
        public string GetUnitName() => Unit.ToString();

        public override bool Equals(object? obj)
            => obj is WeightUnitMeasurable other && Unit == other.Unit;

        public override int GetHashCode() => Unit.GetHashCode();
        public override string ToString() => Unit.ToString();
    }
}