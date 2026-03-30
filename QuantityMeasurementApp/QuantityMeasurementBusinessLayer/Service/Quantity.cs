using System;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC10/UC12/UC13/UC14: Generic Quantity class for any IMeasurable unit.
    /// Implements IMeasurable-based arithmetic and conversion operations.
    /// Lives in BusinessLayer.Service as it implements business logic using IMeasurable interface.
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

        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);

        public Quantity<TUnit> ConvertTo(TUnit targetUnit, int decimalPlaces = 2)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");
            double baseVal   = Unit.ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseVal);
            double rounded   = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        private enum ArithmeticOperation { Add, Subtract, Divide }

        private static void ValidateArithmeticOperands(
            Quantity<TUnit> self, Quantity<TUnit> other,
            TUnit? targetUnit, bool targetUnitRequired)
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

        private double PerformBaseArithmetic(Quantity<TUnit> other, ArithmeticOperation operation)
        {
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

        private Quantity<TUnit> BuildResult(double baseResult, TUnit targetUnit, int decimalPlaces)
        {
            double converted = targetUnit.ConvertFromBaseUnit(baseResult);
            double rounded   = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Add), this.Unit, decimalPlaces);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, targetUnit, targetUnitRequired: true);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Add), targetUnit, decimalPlaces);
        }

        public static Quantity<TUnit> Add(Quantity<TUnit> first, Quantity<TUnit> second,
                                          TUnit resultUnit, int decimalPlaces = 2)
        {
            if (first      == null) throw new ArgumentNullException(nameof(first));
            if (second     == null) throw new ArgumentNullException(nameof(second));
            if (resultUnit == null) throw new ArgumentNullException(nameof(resultUnit));
            return first.Add(second, resultUnit, decimalPlaces);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Subtract), this.Unit, decimalPlaces);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, targetUnit, targetUnitRequired: true);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Subtract), targetUnit, decimalPlaces);
        }

        public double Divide(Quantity<TUnit> other)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return PerformBaseArithmetic(other, ArithmeticOperation.Divide);
        }

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
        public override string ToString()  => $"{Value:F2} {Unit.GetUnitName()}";
    }

    /// <summary>UC15: Internal model wrapping an IMeasurable unit with its value.</summary>
    public class QuantityModel<U> where U : IMeasurable
    {
        public double Value { get; }
        public U      Unit  { get; }

        public QuantityModel(double value, U unit) { Value = value; Unit = unit; }

        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);
        public override string ToString() => $"{Value:G} {Unit.GetUnitName()}";
    }
}
