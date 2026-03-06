using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC9: Generic weight quantity class.
    /// Mirrors the Length design but operates in the Weight measurement category
    /// with KILOGRAM as the base unit.
    /// </summary>
    public class Weight
    {
        /// <summary>
        /// Numeric value of the measurement.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Weight unit (KILOGRAM, GRAM, POUND).
        /// </summary>
        public WeightUnit Unit { get; }

        public Weight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Weight value cannot be NaN");

            if (double.IsInfinity(value))
                throw new ArgumentException("Weight value cannot be Infinity");

            // Allow negative and large values similar to Length; just guard against absurd magnitudes.
            if (Math.Abs(value) > 10000000)
                throw new ArgumentException("Weight value too large. Invalid measurement.");

            if (!Enum.IsDefined(typeof(WeightUnit), unit) || unit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Invalid Weight Unit");

            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Converts this weight to the base unit (kilogram).
        /// </summary>
        public double ToBaseUnit()
        {
            return Unit.ConvertToBaseUnit(Value);
        }

        /// <summary>
        /// Converts this weight to the specified target unit.
        /// Returns a new immutable Weight instance.
        /// </summary>
        public Weight ConvertTo(WeightUnit targetUnit, int decimalPlaces = 2)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit) || targetUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Target unit cannot be UNKNOWN");

            double baseKg = ToBaseUnit();
            double converted = targetUnit.ConvertFromBaseUnit(baseKg);
            double rounded = Math.Round(converted, decimalPlaces);

            return new Weight(rounded, targetUnit);
        }

        /// <summary>
        /// Static helper to convert a numeric value from one unit to another.
        /// </summary>
        public static double Convert(double value, WeightUnit sourceUnit, WeightUnit targetUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            if (!Enum.IsDefined(typeof(WeightUnit), sourceUnit) || sourceUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Source unit is not valid");

            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit) || targetUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Target unit is not valid");

            double baseKg = sourceUnit.ConvertToBaseUnit(value);
            double converted = targetUnit.ConvertFromBaseUnit(baseKg);
            return Math.Round(converted, decimalPlaces);
        }

        /// <summary>
        /// Adds another Weight to this one, returning the result in this instance's unit.
        /// </summary>
        public Weight Add(Weight other, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Second weight cannot be null");

            return Add(this, other, this.Unit, decimalPlaces);
        }

        /// <summary>
        /// Adds another Weight to this one, returning the result in the specified target unit.
        /// </summary>
        public Weight Add(Weight other, WeightUnit resultUnit, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Second weight cannot be null");

            return Add(this, other, resultUnit, decimalPlaces);
        }

        /// <summary>
        /// Static add: sums two Weight objects and returns the result in the target unit.
        /// </summary>
        public static Weight Add(Weight first, Weight second, WeightUnit resultUnit, int decimalPlaces = 2)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first), "First weight cannot be null");

            if (second == null)
                throw new ArgumentNullException(nameof(second), "Second weight cannot be null");

            if (!Enum.IsDefined(typeof(WeightUnit), resultUnit) || resultUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Result unit is not valid");

            double firstBase = first.ToBaseUnit();
            double secondBase = second.ToBaseUnit();
            double sumBase = firstBase + secondBase;

            double sumInResult = resultUnit.ConvertFromBaseUnit(sumBase);
            double rounded = Math.Round(sumInResult, decimalPlaces);

            return new Weight(rounded, resultUnit);
        }

        /// <summary>
        /// Convenience overload that accepts raw values and units.
        /// </summary>
        public static Weight Add(double firstValue, WeightUnit firstUnit,
                                 double secondValue, WeightUnit secondUnit,
                                 WeightUnit resultUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(firstValue) || !double.IsFinite(secondValue))
                throw new ArgumentException("Values must be finite numbers");

            if (!Enum.IsDefined(typeof(WeightUnit), firstUnit) || firstUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("First unit is not valid");

            if (!Enum.IsDefined(typeof(WeightUnit), secondUnit) || secondUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Second unit is not valid");

            Weight first = new Weight(firstValue, firstUnit);
            Weight second = new Weight(secondValue, secondUnit);
            return Add(first, second, resultUnit, decimalPlaces);
        }

        public override bool Equals(object? obj)
        {
            // Category type safety: do not compare to Length or any other type.
            if (obj == null || obj.GetType() != typeof(Weight))
                return false;

            Weight other = (Weight)obj;
            // Compare in base kilograms
            return ToBaseUnit().Equals(other.ToBaseUnit());
        }

        public override int GetHashCode()
        {
            return ToBaseUnit().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value:F2} {Unit}";
        }
    }
}