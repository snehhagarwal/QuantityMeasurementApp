using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: Generic Quantity class that works with any IMeasurable unit.
    /// Replaces the duplicate Length/Weight pattern with a single reusable class.
    ///
    /// UC12: Adds Subtract and Divide operations.
    ///
    /// UC13: Internal refactor only — public API is unchanged.
    ///   Introduces a private ArithmeticOperation enum so each operation
    ///   is just one enum value, not a separate block of duplicated logic.
    ///   All validation lives in ValidateArithmeticOperands.
    ///   All base-unit conversion and computation lives in PerformBaseArithmetic.
    ///   Adding a future operation (e.g. Multiply) requires only one new enum case
    ///   and one new public method — validation and conversion are reused automatically.
    /// </summary>
    public class Quantity<TUnit> where TUnit : IMeasurable
    {
        public double Value { get; }
        public TUnit Unit { get; }

        public Quantity(double value, TUnit unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null");

            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");

            if (Math.Abs(value) > 10_000_000)
                throw new ArgumentException("Value too large. Invalid measurement.");

            Value = value;
            Unit  = unit;
        }

        // ── Conversion ────────────────────────────────────────────────────────

        /// <summary>Returns this quantity expressed in the base unit.</summary>
        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);

        /// <summary>Returns a new Quantity converted to the target unit.</summary>
        public Quantity<TUnit> ConvertTo(TUnit targetUnit, int decimalPlaces = 2)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            double baseValue = ToBaseUnit();
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            double rounded   = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        // ═════════════════════════════════════════════════════════════════════
        // UC13 — Centralized arithmetic infrastructure (all private)
        // ═════════════════════════════════════════════════════════════════════

        // Represents every arithmetic operation the class supports.
        // To add Multiply: add one enum case here + one public method below.
        // Validation and conversion are inherited automatically.
        private enum ArithmeticOperation { Add, Subtract, Divide }

        // Single validation point for all arithmetic operations.
        // Checks null operand, null target unit (when required), and finite values.
        // Cross-category prevention is handled at compile time by TUnit — no runtime check needed.
        private static void ValidateArithmeticOperands(
            Quantity<TUnit> self,
            Quantity<TUnit> other,
            TUnit?          targetUnit,
            bool            targetUnitRequired)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            if (targetUnitRequired && targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            if (!double.IsFinite(self.Value))
                throw new ArgumentException("This quantity value must be finite");

            if (!double.IsFinite(other.Value))
                throw new ArgumentException("Other quantity value must be finite");
        }

        // Single conversion + computation point for all arithmetic operations.
        // Both operands are normalised to the base unit before the operation is applied.
        // Returns a raw base-unit double.
        //   Add / Subtract — caller converts the result back to the target unit via BuildResult.
        //   Divide         — caller returns the raw double directly (dimensionless ratio).
        private double PerformBaseArithmetic(Quantity<TUnit> other, ArithmeticOperation operation)
        {
            double a = this.ToBaseUnit();
            double b = other.ToBaseUnit();

            return operation switch
            {
                ArithmeticOperation.Add      => a + b,
                ArithmeticOperation.Subtract => a - b,
                ArithmeticOperation.Divide   => b == 0.0
                                                    ? throw new ArithmeticException("Cannot divide by a zero quantity")
                                                    : a / b,
                _ => throw new InvalidOperationException($"Unsupported operation: {operation}")
            };
        }

        // Converts a raw base-unit result back to targetUnit, rounds, and wraps it
        // in a new immutable Quantity. Used by Add and Subtract only.
        private Quantity<TUnit> BuildResult(double baseResult, TUnit targetUnit, int decimalPlaces)
        {
            double converted = targetUnit.ConvertFromBaseUnit(baseResult);
            double rounded   = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        // ── Addition ──────────────────────────────────────────────────────────

        /// <summary>Adds another quantity. Result is in this instance's unit.</summary>
        public Quantity<TUnit> Add(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Add), this.Unit, decimalPlaces);
        }

        /// <summary>Adds another quantity. Result is in the specified target unit.</summary>
        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, targetUnit, targetUnitRequired: true);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Add), targetUnit, decimalPlaces);
        }

        /// <summary>Static add — sums two quantities into the specified result unit.</summary>
        public static Quantity<TUnit> Add(Quantity<TUnit> first, Quantity<TUnit> second,
                                          TUnit resultUnit, int decimalPlaces = 2)
        {
            if (first      == null) throw new ArgumentNullException(nameof(first));
            if (second     == null) throw new ArgumentNullException(nameof(second));
            if (resultUnit == null) throw new ArgumentNullException(nameof(resultUnit));

            return first.Add(second, resultUnit, decimalPlaces);
        }

        // ── Subtraction ───────────────────────────────────────────────────────

        /// <summary>Subtracts another quantity. Result is in this instance's unit.</summary>
        public Quantity<TUnit> Subtract(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Subtract), this.Unit, decimalPlaces);
        }

        /// <summary>Subtracts another quantity. Result is in the specified target unit.</summary>
        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, targetUnit, targetUnitRequired: true);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Subtract), targetUnit, decimalPlaces);
        }

        // ── Division ──────────────────────────────────────────────────────────

        /// <summary>
        /// Divides this quantity by another. Returns a dimensionless double ratio.
        /// Throws ArithmeticException if the divisor is zero.
        /// </summary>
        public double Divide(Quantity<TUnit> other)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return PerformBaseArithmetic(other, ArithmeticOperation.Divide);
        }

        // ── Equality ──────────────────────────────────────────────────────────

        /// <summary>
        /// Compares by base-unit value.
        /// Cross-category comparisons (e.g. Length vs Weight) always return false.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            var other = (Quantity<TUnit>)obj;
            if (Unit.GetType() != other.Unit.GetType()) return false;

            return ToBaseUnit().Equals(other.ToBaseUnit());
        }

        public override int GetHashCode() => ToBaseUnit().GetHashCode();

        public override string ToString() => $"{Value:F2} {Unit.GetUnitName()}";
    }
}