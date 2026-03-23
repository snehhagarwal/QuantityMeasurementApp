namespace QuantityMeasurementModel.Units
{
    /// <summary>Value type for inches (domain unit, not an EF entity).</summary>
    public class Inches
    {
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
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || obj.GetType() != typeof(Inches)) return false;
            return Value.CompareTo(((Inches)obj).Value) == 0;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
