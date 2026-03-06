using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: Generic Quantity class for any IMeasurable unit.
    /// UC12: Adds Subtract and Divide.
    /// UC13: Centralized arithmetic via ArithmeticOperation enum, ValidateArithmeticOperands, PerformBaseArithmetic.
    /// UC14: Two targeted additions:
    ///   1. PerformBaseArithmetic calls unit.ValidateOperationSupport(operationName) before any calculation
    ///      — temperature throws NotSupportedException here.
    ///   2. Equals uses epsilon tolerance (1e-9) for floating-point safe comparison
    ///      — needed for temperature conversions (e.g. 0°C == 32°F after round-trip).
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

        /// <summary>
        /// Returns a new Quantity converted to the target unit.
        /// UC14: Goes through ConvertToBaseUnit → ConvertFromBaseUnit, which handles
        /// both linear (Length/Weight/Volume) and non-linear (Temperature) units correctly.
        /// </summary>
        public Quantity<TUnit> ConvertTo(TUnit targetUnit, int decimalPlaces = 2)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            // UC14: For temperature, this is the correct two-step path.
            // For linear units it produces the same result as before.
            double baseCelsius = Unit.ConvertToBaseUnit(Value);
            double converted   = targetUnit.ConvertFromBaseUnit(baseCelsius);
            double rounded     = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        // ═════════════════════════════════════════════════════════════════════
        // UC13 — Centralized arithmetic infrastructure (all private)
        // ═════════════════════════════════════════════════════════════════════

        // Represents every arithmetic operation the class supports.
        private enum ArithmeticOperation { Add, Subtract, Divide }

        // Single validation point for null / finite checks.
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

        /// <summary>
        /// Converts both operands to base unit and applies the operation.
        /// UC14: Calls unit.ValidateOperationSupport before any numbers are touched.
        ///       Temperature overrides this to throw NotSupportedException immediately.
        /// </summary>
        private double PerformBaseArithmetic(Quantity<TUnit> other, ArithmeticOperation operation)
        {
            // UC14: check whether this unit category supports the requested operation.
            // Length/Weight/Volume do nothing here. Temperature throws NotSupportedException.
            this.Unit.ValidateOperationSupport(operation.ToString());

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

        // Wraps a raw base-unit result back to targetUnit and rounds.
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
        /// Throws NotSupportedException if the unit does not support arithmetic (e.g. temperature).
        /// Throws ArithmeticException if the divisor is zero.
        /// </summary>
        public double Divide(Quantity<TUnit> other)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return PerformBaseArithmetic(other, ArithmeticOperation.Divide);
        }

        // ── Equality ──────────────────────────────────────────────────────────

        /// <summary>
        /// Compares by base-unit value using epsilon tolerance.
        /// UC14: Epsilon is needed for temperature — floating-point round-trips introduce
        ///       tiny errors (e.g. 0°C → 32°F → 0°C may give 1e-14 difference).
        /// Cross-category comparisons (e.g. Temperature vs Length) always return false.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            var other = (Quantity<TUnit>)obj;
            if (Unit.GetType() != other.Unit.GetType()) return false;

            const double epsilon = 1e-9;
            return Math.Abs(ToBaseUnit() - other.ToBaseUnit()) < epsilon;
        }

        public override int GetHashCode() => ToBaseUnit().GetHashCode();

        public override string ToString() => $"{Value:F2} {Unit.GetUnitName()}";
    }
}