using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC11: IMeasurable wrapper for VolumeUnit.
    /// Bridges the VolumeUnit enum and its extension methods to the IMeasurable
    /// interface required by Quantity&lt;TUnit&gt;.
    /// Mirrors the structure of LengthUnitMeasurable and WeightUnitMeasurable
    /// established in UC10 — no changes to IMeasurable or Quantity&lt;TUnit&gt; needed.
    /// </summary>
    public readonly struct VolumeUnitMeasurable : IMeasurable
    {
        public VolumeUnit Unit { get; }

        public VolumeUnitMeasurable(VolumeUnit unit)
        {
            Unit = unit;
        }

        public double GetConversionFactor()             => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value)   => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double baseValue) => Unit.ConvertFromBaseUnit(baseValue);
        public string GetUnitName()                     => Unit.ToString();

        public override bool Equals(object? obj)
            => obj is VolumeUnitMeasurable other && Unit == other.Unit;

        public override int GetHashCode() => Unit.GetHashCode();
        public override string ToString() => Unit.ToString();
    }
}