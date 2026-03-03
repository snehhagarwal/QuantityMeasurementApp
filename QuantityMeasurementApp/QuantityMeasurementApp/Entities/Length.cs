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
        /// 
        /// UC5: Negative values are supported for conversion operations.
        /// Negative measurements convert correctly while preserving sign.
        /// </summary>
        public Length(double value, LengthUnit unit)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Value cannot be NaN");

            if (double.IsInfinity(value))
                throw new ArgumentException("Value cannot be Infinity");

            // UC5: Negative values are allowed for conversion operations
            // Removed negative check to support UC5 requirement: "Negative measurements convert correctly while preserving sign"

            // Allow reasonably large magnitudes for UC6 large value tests.
            // This limit only protects from extreme overflow values.
            if (Math.Abs(value) > 10000000)
            {
                throw new ArgumentException("Length value too large. Invalid measurement.");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), unit) || unit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Invalid Length Unit");

            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// UC8: Converts measurement to base unit FEET.
        /// Conversion responsibility is delegated to the unit (see LengthUnitExtensions).
        /// </summary>
        public double ToBaseUnit()
        {
            return Unit.ConvertToBaseUnit(Value);
        }

        /// <summary>
        /// UC5: Converts this length to the specified target unit.
        /// Returns a new Length instance representing the same physical length in the target unit.
        /// This method preserves immutability by returning a new instance rather than modifying the current one.
        /// 
        /// Value object semantics and immutability: This means that instances are treated as immutable values.
        /// They are used to represent a specific length measurement and do not change state after creation.
        /// Hence the convertTo method returns a new instance rather than modifying the existing one.
        /// </summary>
        /// <param name="targetUnit">The unit to convert this length into; must not be null or UNKNOWN</param>
        /// <param name="decimalPlaces">Optional precision parameter. Default is 2 decimal places. UC5 requirement: "caller-specified or a default epsilon"</param>
        /// <returns>A new Length object representing the same physical length in the targetUnit, rounded to specified decimal places</returns>
        /// <exception cref="ArgumentException">Thrown if targetUnit is null, UNKNOWN, or invalid</exception>
        public Length ConvertTo(LengthUnit targetUnit, int decimalPlaces = 2)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit) || targetUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Target unit cannot be null or UNKNOWN");

            // Convert to base unit (feet) and then to target unit via unit conversion methods
            double baseUnitValueInFeet = ToBaseUnit();
            double convertedValue = targetUnit.ConvertFromBaseUnit(baseUnitValueInFeet);

            // UC5: Apply optional rounding or precision handling (caller-specified or a default epsilon)
            double roundedValue = Math.Round(convertedValue, decimalPlaces);

            return new Length(roundedValue, targetUnit);
        }

        /// <summary>
        /// UC5: Static method to convert a numeric value from one unit to another.
        /// This method validates inputs and performs conversion using the base unit (inches).
        /// </summary>
        /// <param name="value">The numeric value to convert; must be finite. Negative values are allowed.</param>
        /// <param name="sourceUnit">The source unit; must not be UNKNOWN</param>
        /// <param name="targetUnit">The target unit; must not be UNKNOWN</param>
        /// <param name="decimalPlaces">Optional precision parameter. Default is 2 decimal places.</param>
        /// <returns>The converted numeric value in the target unit, rounded to the given number of decimal places.</returns>
        /// <exception cref="ArgumentException">Thrown if value is NaN, infinite, or units are invalid.</exception>
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit) || sourceUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Source unit is not valid");

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit) || targetUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Target unit is not valid");

            // Convert to base unit (feet) and then to target unit via unit conversion methods
            double baseUnitValueInFeet = sourceUnit.ConvertToBaseUnit(value);
            double convertedValue = targetUnit.ConvertFromBaseUnit(baseUnitValueInFeet);

            return Math.Round(convertedValue, decimalPlaces);
        }

        /// <summary>
        /// UC6: Adds another length to this one.
        /// The result is in the unit of this instance.
        /// A new Length is returned. The current objects are not changed.
        /// </summary>
        /// <param name="other">The second length to add.</param>
        /// <param name="decimalPlaces">Decimal places used for rounding. Default is 2.</param>
        /// <returns>New Length that holds the sum.</returns>
        /// <exception cref="ArgumentNullException">Thrown if other is null.</exception>
        public Length Add(Length other, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Second length cannot be null");

            // Use static Add so that all add logic is in one place
            return Add(this, other, this.Unit, decimalPlaces);
        }

        /// <summary>
        /// UC8: Adds another length to this one and returns the result
        /// in the explicitly specified result unit.
        /// Convenience instance wrapper over the static Add method.
        /// </summary>
        /// <param name="other">The second length to add.</param>
        /// <param name="resultUnit">Target unit for the result.</param>
        /// <param name="decimalPlaces">Decimal places used for rounding. Default is 2.</param>
        /// <returns>New Length in the requested result unit.</returns>
        public Length Add(Length other, LengthUnit resultUnit, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Second length cannot be null");

            return Add(this, other, resultUnit, decimalPlaces);
        }

        /// <summary>
        /// UC6: Adds two Length objects and returns the result in the given result unit.
        /// Both inputs stay unchanged.
        /// </summary>
        /// <param name="first">First length.</param>
        /// <param name="second">Second length.</param>
        /// <param name="resultUnit">Unit for the result.</param>
        /// <param name="decimalPlaces">Decimal places used for rounding. Default is 2.</param>
        /// <returns>New Length with the sum of the two lengths.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any length is null.</exception>
        /// <exception cref="ArgumentException">Thrown if result unit is not valid.</exception>
        public static Length Add(Length first, Length second, LengthUnit resultUnit, int decimalPlaces = 2)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first), "First length cannot be null");

            if (second == null)
                throw new ArgumentNullException(nameof(second), "Second length cannot be null");

            if (!Enum.IsDefined(typeof(LengthUnit), resultUnit) || resultUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Result unit is not valid");

            // Convert both to base unit (feet)
            double firstInBase = first.ToBaseUnit();
            double secondInBase = second.ToBaseUnit();

            double sumInBase = firstInBase + secondInBase;

            // Convert back to result unit
            double sumInResultUnit = resultUnit.ConvertFromBaseUnit(sumInBase);

            double rounded = Math.Round(sumInResultUnit, decimalPlaces);
            return new Length(rounded, resultUnit);
        }

        /// <summary>
        /// UC6: Adds two raw length values with units and returns the sum in the given result unit.
        /// </summary>
        /// <param name="firstValue">First numeric value.</param>
        /// <param name="firstUnit">Unit of the first value.</param>
        /// <param name="secondValue">Second numeric value.</param>
        /// <param name="secondUnit">Unit of the second value.</param>
        /// <param name="resultUnit">Unit for the result.</param>
        /// <param name="decimalPlaces">Decimal places used for rounding. Default is 2.</param>
        /// <returns>New Length with the sum of the two inputs.</returns>
        public static Length Add(double firstValue, LengthUnit firstUnit,
                                 double secondValue, LengthUnit secondUnit,
                                 LengthUnit resultUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(firstValue) || !double.IsFinite(secondValue))
                throw new ArgumentException("Values must be finite numbers");

            if (!Enum.IsDefined(typeof(LengthUnit), firstUnit) || firstUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("First unit is not valid");

            if (!Enum.IsDefined(typeof(LengthUnit), secondUnit) || secondUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Second unit is not valid");

            // Create Length objects so we reuse all validation in the constructor
            Length first = new Length(firstValue, firstUnit);
            Length second = new Length(secondValue, secondUnit);

            return Add(first, second, resultUnit, decimalPlaces);
        }

        /// <summary>
        /// Compares equality between two Length objects.
        /// </summary>
        public override bool Equals(object? obj)
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

        /// <summary>
        /// UC5: Returns a string representation of this Length.
        /// Format: "{value} {unit}" where value is formatted to two decimal places.
        /// Example: "12.00 INCHES", "3.50 FEET"
        /// </summary>
        /// <returns>A formatted string representation of this length</returns>
        public override string ToString()
        {
            return $"{Value:F2} {Unit}";
        }
    }
}