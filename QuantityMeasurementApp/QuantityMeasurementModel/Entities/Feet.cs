using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Entity class representing Feet measurement.
    /// Lives in QuantityMeasurementModel.Entities: pure data entity with no business-layer dependencies.
    /// </summary>
    public class Feet
    {
        /// <summary>Gets the measurement value in feet. Immutable after creation.</summary>
        public double Value { get; }

        public Feet(double value)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Feet value cannot be NaN");
            if (double.IsInfinity(value))
                throw new ArgumentException("Feet value cannot be Infinity");
            if (value < 0)
                throw new ArgumentException("Feet value cannot be negative");
            if (value > 100000)
                throw new ArgumentException("Feet value too large. Invalid measurement.");

            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (obj.GetType() != typeof(Feet)) return false;
            return this.Value.CompareTo(((Feet)obj).Value) == 0;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
