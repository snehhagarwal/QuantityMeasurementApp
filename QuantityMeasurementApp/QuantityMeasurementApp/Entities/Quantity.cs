using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: Generic Quantity class that works with any IMeasurable unit.
    /// Replaces the duplicate Length/Weight pattern with a single reusable class.
    /// Existing Length and Weight classes are preserved for backward compatibility.
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
            Unit = unit;
        }

        /// <summary>Converts this quantity to the base unit.</summary>
        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);

        /// <summary>Converts to the target unit, returns a new immutable Quantity.</summary>
        public Quantity<TUnit> ConvertTo(TUnit targetUnit, int decimalPlaces = 2)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");

            double baseValue = ToBaseUnit();
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            double rounded = Math.Round(converted, decimalPlaces);

            return new Quantity<TUnit>(rounded, targetUnit);
        }

        /// <summary>Adds another quantity, result in this instance's unit.</summary>
        public Quantity<TUnit> Add(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");

            return Add(this, other, this.Unit, decimalPlaces);
        }

        /// <summary>Adds another quantity, result in the specified target unit.</summary>
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
            if (first == null)  throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (resultUnit == null) throw new ArgumentNullException(nameof(resultUnit));

            double sumBase = first.ToBaseUnit() + second.ToBaseUnit();
            double sumInResult = resultUnit.ConvertFromBaseUnit(sumBase);
            double rounded = Math.Round(sumInResult, decimalPlaces);

            return new Quantity<TUnit>(rounded, resultUnit);
        }

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

            return ToBaseUnit().Equals(other.ToBaseUnit());        }

        public override int GetHashCode() => ToBaseUnit().GetHashCode();

        public override string ToString() => $"{Value:F2} {Unit.GetUnitName()}";
    }
}