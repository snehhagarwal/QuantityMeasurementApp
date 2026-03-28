namespace QuantityMeasurementModel.Enums
{
    /// <summary>
    /// UC17 Step 5D: OperationType enum — type-safe representation of all
    /// supported quantity measurement operations.
    ///
    /// Java equivalent: the OperationType enum described in Step 5D of the spec.
    /// Used in QuantityMeasurement entity and QuantityMeasurementDto to ensure
    /// only valid operation types are used throughout the application.
    /// </summary>
    public enum OperationType
    {
        COMPARE,
        CONVERT,
        ADD,
        SUBTRACT,
        DIVIDE
    }
}
