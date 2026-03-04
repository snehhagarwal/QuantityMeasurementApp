using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: Generic Quantity class that works with any IMeasurable unit.
    /// Replaces the duplicate Length/Weight pattern with a single reusable class.
    /// Existing Length and Weight classes are preserved for backward compatibility.
    ///
    /// UC12: Adds Subtract and Divide operations.
    ///   Subtract — returns a new Quantity (same type, immutable).
    ///   Divide   — returns a dimensionless double (the ratio between two quantities).
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

        /// <summary>Converts this quantity to the base unit.</summary>
        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);

        // ── Conversion ────────────────────────────────────────────────────────

        /// <summary>Converts to the target unit and returns a new immutable Quantity.</summary>
        public Quantity<TUnit> ConvertTo(TUnit targetUnit, int decimalPlaces = 2)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            double baseValue = ToBaseUnit();
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            double rounded   = Math.Round(converted, decimalPlaces);

            return new Quantity<TUnit>(rounded, targetUnit);
        }

        // ── Addition ──────────────────────────────────────────────────────────

        /// <summary>Adds another quantity; result in this instance's unit.</summary>
        public Quantity<TUnit> Add(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            return Add(this, other, this.Unit, decimalPlaces);
        }

        /// <summary>Adds another quantity; result in the specified target unit.</summary>
        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            return Add(this, other, targetUnit, decimalPlaces);
        }

        /// <summary>Static add — sums two quantities into the specified result unit.</summary>
        public static Quantity<TUnit> Add(Quantity<TUnit> first, Quantity<TUnit> second,
                                          TUnit resultUnit, int decimalPlaces = 2)
        {
            if (first      == null) throw new ArgumentNullException(nameof(first));
            if (second     == null) throw new ArgumentNullException(nameof(second));
            if (resultUnit == null) throw new ArgumentNullException(nameof(resultUnit));

            double sumBase     = first.ToBaseUnit() + second.ToBaseUnit();
            double sumInResult = resultUnit.ConvertFromBaseUnit(sumBase);
            double rounded     = Math.Round(sumInResult, decimalPlaces);

            return new Quantity<TUnit>(rounded, resultUnit);
        }

        // ── Subtraction (UC12) ────────────────────────────────────────────────

        /// <summary>
        /// Subtracts another quantity from this quantity.
        /// Result is returned in this instance's unit (implicit target).
        /// Negative results are valid — they mean the other quantity is larger.
        /// </summary>
        public Quantity<TUnit> Subtract(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            return Subtract(this, other, this.Unit, decimalPlaces);
        }

        /// <summary>
        /// Subtracts another quantity from this quantity.
        /// Result is returned in the specified target unit.
        /// </summary>
        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            return Subtract(this, other, targetUnit, decimalPlaces);
        }

        /// <summary>
        /// Static subtract — subtracts second from first and expresses the result in resultUnit.
        /// Both quantities must belong to the same measurement category (enforced by TUnit).
        /// </summary>
        public static Quantity<TUnit> Subtract(Quantity<TUnit> first, Quantity<TUnit> second,
                                                TUnit resultUnit, int decimalPlaces = 2)
        {
            if (first      == null) throw new ArgumentNullException(nameof(first));
            if (second     == null) throw new ArgumentNullException(nameof(second));
            if (resultUnit == null) throw new ArgumentNullException(nameof(resultUnit));

            double diffBase     = first.ToBaseUnit() - second.ToBaseUnit();
            double diffInResult = resultUnit.ConvertFromBaseUnit(diffBase);
            double rounded      = Math.Round(diffInResult, decimalPlaces);

            return new Quantity<TUnit>(rounded, resultUnit);
        }

        // ── Division (UC12) ───────────────────────────────────────────────────

        /// <summary>
        /// Divides this quantity by another quantity of the same category.
        /// Returns a dimensionless double — the ratio of the two measurements.
        ///
        ///   result > 1.0  → this quantity is larger than other
        ///   result = 1.0  → quantities are equivalent
        ///   result &lt; 1.0  → this quantity is smaller than other
        ///
        /// Throws ArithmeticException if the divisor quantity is zero.
        /// </summary>
        public double Divide(Quantity<TUnit> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Divisor quantity cannot be null");

            double divisorBase = other.ToBaseUnit();

            if (divisorBase == 0.0)
                throw new ArithmeticException("Cannot divide by a zero quantity");

            return this.ToBaseUnit() / divisorBase;
        }

        // ── Equality ──────────────────────────────────────────────────────────

        /// <summary>
        /// Equality compares base-unit values.
        /// Cross-category comparisons (e.g. Length vs Weight) return false.
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