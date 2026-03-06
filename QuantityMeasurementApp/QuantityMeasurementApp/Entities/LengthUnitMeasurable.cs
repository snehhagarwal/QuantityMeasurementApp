using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: IMeasurable wrapper for LengthUnit.
    /// Bridges the existing LengthUnit enum and extension methods
    /// to the IMeasurable interface required by Quantity<TUnit>.
    /// </summary>
    public readonly struct LengthUnitMeasurable : IMeasurable
    {
        public LengthUnit Unit { get; }

        public LengthUnitMeasurable(LengthUnit unit)
        {
            Unit = unit;
        }

        public double GetConversionFactor() => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double baseValue) => Unit.ConvertFromBaseUnit(baseValue);
        public string GetUnitName() => Unit.ToString();

        public override bool Equals(object? obj)
            => obj is LengthUnitMeasurable other && Unit == other.Unit;

        public override int GetHashCode() => Unit.GetHashCode();
        public override string ToString() => Unit.ToString();
    }
}