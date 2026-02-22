using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// Generic Quantity Length class.
    /// Implements DRY principle by replacing Feet and Inches duplicate logic.
    /// </summary>
    public class Length
    {
        /// <summary>
        /// Numeric value of measurement.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Unit of measurement (Feet or Inches).
        /// </summary>
        public LengthUnit Unit { get; }

        /// <summary>
        /// Constructor initializes value and unit.
        /// Performs validation.
        /// </summary>
        public Length(double value, LengthUnit unit)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Value cannot be NaN");

            if (double.IsInfinity(value))
                throw new ArgumentException("Value cannot be Infinity");

            if (value < 0)
                throw new ArgumentException("Value cannot be negative");

            if (value > 100000)
            {
                throw new ArgumentException("Length value too large. Invalid measurement.");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), unit) || unit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Invalid Length Unit");

            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Converts measurement to base unit Inches.
        /// </summary>
        public double ToBaseUnit()
        {
            switch (Unit)
            {
                case LengthUnit.FEET:
                    return Value * 12;

                case LengthUnit.INCHES:
                    return Value;

                case LengthUnit.YARDS:
                    return Value * 36;

                case LengthUnit.CENTIMETERS:
                    return Value * 0.393701;

                default:
                    throw new ArgumentException("Invalid unit");
            }
        }

        /// <summary>
        /// Compares equality between two Length objects.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Length))
                return false;

            Length other = (Length)obj;

            return this.ToBaseUnit() == other.ToBaseUnit();
        }

        public override int GetHashCode()
        {
            return ToBaseUnit().GetHashCode();
        }
    }
}