namespace QuantityMeasurementBusinessLayer.Exceptions
{
    /// <summary>
    /// UC17 MVC — domain exception for invalid or unsupported quantity operations.
    /// Lives inside the MVC project; does not depend on any external assembly.
    ///
    /// Thrown by QuantityMeasurementService for:
    ///   - Cross-category operations (LENGTH + WEIGHT)
    ///   - Temperature arithmetic
    ///   - Unknown unit names
    ///   - Null inputs
    /// </summary>
    public class QuantityMeasurementException : ApplicationException
    {
        public QuantityMeasurementException(string message) : base(message) { }

        public QuantityMeasurementException(string message, Exception inner)
            : base(message, inner) { }
    }
}
