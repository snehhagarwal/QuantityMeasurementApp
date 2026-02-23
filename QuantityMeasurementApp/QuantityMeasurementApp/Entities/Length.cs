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

            if (Math.Abs(value) > 100000)
            {
                throw new ArgumentException("Length value too large. Invalid measurement.");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), unit) || unit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Invalid Length Unit");

            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// UC5: Converts measurement to base unit Inches.
        /// Uses the enum conversion factor pattern as required by UC5.
        /// </summary>
        public double ToBaseUnit()
        {
            // UC5: Use enum extension method for conversion factor
            return Value * Unit.GetConversionFactor();
        }

        /// <summary>
        /// UC5: Private utility method: Gets conversion factor from base unit (inches) to target unit.
        /// Uses the enum conversion factor pattern as required by UC5.
        /// </summary>
        /// <param name="targetUnit">The target unit to convert to</param>
        /// <returns>Conversion factor (inches per target unit)</returns>
        private double GetConversionFactorFromBase(LengthUnit targetUnit)
        {
            // UC5: Use enum extension method for conversion factor
            return targetUnit.GetConversionFactor();
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

            // Convert to base unit (inches) using enum conversion factor
            double baseUnitValue = ToBaseUnit();

            // Convert from base unit to target unit using enum conversion factor
            double conversionFactor = GetConversionFactorFromBase(targetUnit);
            double convertedValue = baseUnitValue / conversionFactor;

            // UC5: Apply optional rounding or precision handling (caller-specified or a default epsilon)
            double roundedValue = Math.Round(convertedValue, decimalPlaces);

            return new Length(roundedValue, targetUnit);
        }

        /// <summary>
        /// UC5: Static method to convert a numeric value from one unit to another.
        /// This method validates inputs and performs conversion using the base unit (inches).
        /// 
        /// Uses the enum conversion factor pattern: sourceUnit.getConversionFactor() and targetUnit.getConversionFactor()
        /// as required by UC5.
        /// 
        /// Conversion formula: result = value × (sourceUnit.factor / targetUnit.factor)
        /// The formula consistently produces correct results across all unit pairs.
        /// </summary>
        /// <param name="value">The numeric value to convert; must be finite. UC5: Supports negative values, preserving sign during conversion.</param>
        /// <param name="sourceUnit">The source unit; must not be null or UNKNOWN</param>
        /// <param name="targetUnit">The target unit; must not be null or UNKNOWN</param>
        /// <param name="decimalPlaces">Optional precision parameter. Default is 2 decimal places. UC5 requirement: "caller-specified or a default epsilon"</param>
        /// <returns>The converted numeric value in the target unit, rounded to specified decimal places</returns>
        /// <exception cref="ArgumentException">Thrown if value is NaN, infinite, or units are invalid</exception>
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit) || sourceUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Source unit cannot be null or UNKNOWN");

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit) || targetUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Target unit cannot be null or UNKNOWN");

            // UC5: Use enum conversion factors directly for conversion
            // Convert to base unit using source unit's conversion factor
            double baseUnitValue = value * sourceUnit.GetConversionFactor();
            
            // Convert from base unit to target unit using target unit's conversion factor
            double targetUnitFactor = targetUnit.GetConversionFactor();
            double convertedValue = baseUnitValue / targetUnitFactor;

            // UC5: Apply optional rounding or precision handling (caller-specified or a default epsilon)
            return Math.Round(convertedValue, decimalPlaces);
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