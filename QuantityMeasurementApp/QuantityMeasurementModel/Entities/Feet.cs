using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Entity class representing Feet measurement.
    /// This class stores value and performs equality comparison.
    /// Includes validation for invalid inputs like NaN and Infinity.
    /// </summary>
    public class Feet
    {
        /// <summary>
        /// Gets the measurement value in feet.
        /// Immutable property (cannot change after creation).
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Constructor to initialize Feet value.
        /// Throws exception if invalid value entered.
        /// </summary>
        /// <param name="value">Measurement in feet</param>
        public Feet(double value)
        {
            // Check NaN
            if (double.IsNaN(value))
                throw new ArgumentException("Feet value cannot be NaN");

            // Check Infinity
            if (double.IsInfinity(value))
                throw new ArgumentException("Feet value cannot be Infinity");
            
            if (value < 0)
                throw new ArgumentException("Feet value cannot be negative");
            
            if (value > 100000)
                throw new ArgumentException("Feet value too large. Invalid measurement.");

            Value = value;
        }

        /// <summary>
        /// Overrides Equals method for value-based comparison.
        /// Checks if two Feet objects have same value.
        /// </summary>
        public override bool Equals(object? obj)
        {
            // Same reference
            if (this == obj)
                return true;

            // Null check
            if (obj == null)
                return false;

            // Type check
            if (obj.GetType() != typeof(Feet))
                return false;

            Feet other = (Feet)obj;

            // Compare values
            return this.Value.CompareTo(other.Value) == 0;
        }

        /// <summary>
        /// Override GetHashCode when Equals overridden.
        /// Used in collections like Dictionary.
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}