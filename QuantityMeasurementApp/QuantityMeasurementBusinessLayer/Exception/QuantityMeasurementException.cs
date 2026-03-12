using System;

namespace QuantityMeasurementBusinessLayer.Exception
{
    /// <summary>
    /// UC15: Custom unchecked exception (extends ApplicationException = RuntimeException in C#).
    ///
    /// Thrown by QuantityMeasurementServiceImpl when:
    ///   - Invalid measurements are provided (null, NaN, infinite)
    ///   - Cross-category operations attempted (e.g. LENGTH + WEIGHT)
    ///   - Unsupported arithmetic operations requested (e.g. Temperature Add/Subtract/Divide)
    ///   - Unit conversion fails due to unknown unit names
    ///   - Division by zero occurs
    ///
    /// Being unchecked (ApplicationException) means callers are not forced to catch it,
    /// while still providing the option to handle it specifically.
    /// </summary>
    public class QuantityMeasurementException : ApplicationException
    {
        /// <summary>Creates exception with a descriptive message.</summary>
        public QuantityMeasurementException(string message)
            : base(message) { }

        /// <summary>Creates exception wrapping an inner exception for chaining.</summary>
        public QuantityMeasurementException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
