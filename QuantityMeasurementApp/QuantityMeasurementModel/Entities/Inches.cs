using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Entity class representing Inches measurement.
    /// Lives in QuantityMeasurementModel.Entities: pure data entity with no business-layer dependencies.
    /// </summary>
    public class Inches
    {
        /// <summary>Gets the measurement value in inches. Immutable after creation.</summary>
        public double Value { get; }

        public Inches(double measurementValue)
        {
            if (double.IsNaN(measurementValue))
                throw new ArgumentException("Inches value cannot be NaN");
            if (double.IsInfinity(measurementValue))
                throw new ArgumentException("Inches value cannot be Infinity");
            if (measurementValue < 0)
                throw new ArgumentException("Inches value cannot be negative");
            if (measurementValue > 100000)
                throw new ArgumentException("Inches value too large. Invalid measurement.");

            Value = measurementValue;
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (obj.GetType() != typeof(Inches)) return false;
            return this.Value.CompareTo(((Inches)obj).Value) == 0;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
