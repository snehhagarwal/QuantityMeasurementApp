namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// UC10: Contract that all measurement unit wrappers must implement.
    /// UC14: Refactored to support optional arithmetic operations via default methods.
    /// UC15: Added GetMeasurementType() helper to support QuantityDTO mapping in ServiceImpl.
    /// </summary>
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns display name of the unit e.g. "FEET", "KILOGRAM".</summary>
        string GetUnitName();

        /// <summary>Returns category e.g. "LENGTH", "WEIGHT", "VOLUME", "TEMPERATURE".</summary>
        string GetMeasurementType();

        public delegate bool SupportsArithmetic();

        public bool SupportsArithmeticOps() => true;

        public void ValidateOperationSupport(string operation) { }
    }
}
