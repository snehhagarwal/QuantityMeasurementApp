using System;

namespace QuantityMeasurementRepository.Exception
{
    /// <summary>
    /// UC16: Custom exception for database-related errors.
    /// Wraps SQLite/ADO.NET exceptions to provide meaningful context
    /// to upper layers without exposing low-level database details.
    ///
    /// Equivalent to Java's DatabaseException extending RuntimeException.
    /// Being unchecked (ApplicationException) means callers are not forced to catch it.
    /// </summary>
    public class DatabaseException : ApplicationException
    {
        /// <summary>Creates exception with a descriptive message.</summary>
        public DatabaseException(string message)
            : base(message) { }

        /// <summary>Creates exception wrapping an inner database exception.</summary>
        public DatabaseException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
