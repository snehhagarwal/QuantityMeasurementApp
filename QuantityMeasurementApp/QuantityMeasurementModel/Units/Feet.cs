namespace QuantityMeasurementModel.Units
{
    /// <summary>Value type for feet (domain unit, not an EF entity).</summary>
    public class Feet
    {
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
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || obj.GetType() != typeof(Feet)) return false;
            return Value.CompareTo(((Feet)obj).Value) == 0;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
