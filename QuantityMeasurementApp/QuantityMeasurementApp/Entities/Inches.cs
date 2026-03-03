using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// Entity class representing Inches measurement.
    /// This class stores value and performs equality comparison.
    /// Includes validation for invalid inputs like NaN and Infinity.
    /// </summary>
    public class Inches
    {
        /// <summary>
        /// Gets the measurement value in inches.
        /// Immutable property (cannot change after creation).
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Constructor to initialize Inches value.
        /// Throws exception if invalid value entered.
        /// </summary>
        /// <param name="measurementValue">Measurement in inches</param>
        public Inches(double measurementValue)
        {
            // Check NaN
            if (double.IsNaN(measurementValue))
                throw new ArgumentException("Inches value cannot be NaN");

            // Check Infinity
            if (double.IsInfinity(measurementValue))
                throw new ArgumentException("Inches value cannot be Infinity");

            // Check negative
            if (measurementValue < 0)
                throw new ArgumentException("Inches value cannot be negative");

            // Check very large value
            if (measurementValue > 100000)
                throw new ArgumentException("Inches value too large. Invalid measurement.");

            Value = measurementValue;
        }

        /// <summary>
        /// Overrides Equals method for value-based comparison.
        /// Checks if two Inches objects have same value.
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
            if (obj.GetType() != typeof(Inches))
                return false;

            Inches otherMeasurement = (Inches)obj;

            // Compare values
            return this.Value.CompareTo(otherMeasurement.Value) == 0;
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