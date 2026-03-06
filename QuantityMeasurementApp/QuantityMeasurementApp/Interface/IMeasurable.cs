namespace QuantityMeasurementApp.Interface
{
    /// <summary>
    /// UC10: Contract that all measurement unit wrappers must implement.
    ///
    /// UC14: Refactored to support optional arithmetic operations via default methods.
    ///   Existing units (Length, Weight, Volume) require zero changes — they inherit
    ///   the defaults which allow all operations.
    ///   TemperatureUnitMeasurable overrides the defaults to block arithmetic.
    /// </summary>
    public interface IMeasurable
    {
        // ── Mandatory conversion methods ──────────────────────────────────────

        /// <summary>Returns the conversion factor relative to the base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value in this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a base-unit value back to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns the display name of this unit.</summary>
        string GetUnitName();

        // ── UC14: Functional interface ────────────────────────────────────────

        /// <summary>
        /// Functional interface with a single method that reports whether a unit
        /// category supports arithmetic operations.
        /// Lambda usage: SupportsArithmetic supportsArithmetic = () => true;
        /// </summary>
        public delegate bool SupportsArithmetic();

        // ── UC14: Default methods (inherited by all existing units automatically) ──

        /// <summary>
        /// Returns true if this unit supports arithmetic operations.
        /// Default: true — Length, Weight, Volume inherit this unchanged.
        /// TemperatureUnitMeasurable overrides to return false.
        /// </summary>
        public bool SupportsArithmeticOps() => true;

        /// <summary>
        /// Validates that the named operation is supported by this unit.
        /// Default: does nothing — all existing units allow all arithmetic operations.
        /// TemperatureUnitMeasurable overrides to throw NotSupportedException.
        /// </summary>
        public void ValidateOperationSupport(string operation) { }
    }
}