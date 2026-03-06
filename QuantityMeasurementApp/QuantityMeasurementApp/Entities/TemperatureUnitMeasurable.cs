using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC14: IMeasurable wrapper for TemperatureUnit.
    /// Mirrors the structure of LengthUnitMeasurable, WeightUnitMeasurable, VolumeUnitMeasurable.
    ///
    /// Key difference: temperature does not support arithmetic operations.
    /// Adding/dividing absolute temperatures is physically meaningless.
    ///
    /// Lambda expression implements the SupportsArithmetic functional interface from IMeasurable:
    ///   IMeasurable.SupportsArithmetic supportsArithmetic = () => false;
    ///
    /// SupportsArithmeticOps() returns false.
    /// ValidateOperationSupport() throws NotSupportedException for any arithmetic attempt.
    /// </summary>
    public readonly struct TemperatureUnitMeasurable : IMeasurable
    {
        public TemperatureUnit Unit { get; }

        // Lambda expression implementing the SupportsArithmetic functional interface.
        // Returns false — temperature does not support arithmetic.
        private static readonly IMeasurable.SupportsArithmetic supportsArithmetic = () => false;

        public TemperatureUnitMeasurable(TemperatureUnit unit)
        {
            Unit = unit;
        }

        // ── IMeasurable mandatory methods ─────────────────────────────────────

        public double GetConversionFactor()             => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value)   => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double baseValue) => Unit.ConvertFromBaseUnit(baseValue);
        public string GetUnitName()                     => Unit.ToString();

        // ── UC14: Override default methods to block arithmetic ────────────────

        /// <summary>
        /// Returns false — delegates to the SupportsArithmetic lambda.
        /// Called by Quantity&lt;TUnit&gt; before any arithmetic operation.
        /// </summary>
        public bool SupportsArithmeticOps() => supportsArithmetic();

        /// <summary>
        /// Throws NotSupportedException for any arithmetic operation on temperature.
        /// Called by Quantity&lt;TUnit&gt;.PerformBaseArithmetic before calculation.
        /// </summary>
        public void ValidateOperationSupport(string operation)
        {
            throw new NotSupportedException($"Temperature does not support {operation}.");
        }

        // ── Equality ──────────────────────────────────────────────────────────

        public override bool Equals(object? obj)
            => obj is TemperatureUnitMeasurable other && Unit == other.Unit;

        public override int GetHashCode() => Unit.GetHashCode();
        public override string ToString()  => Unit.ToString();
    }
}