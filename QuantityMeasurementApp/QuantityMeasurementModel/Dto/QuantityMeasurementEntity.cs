using System;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// UC15: Immutable POJO entity representing a complete quantity measurement operation record.
    ///
    /// Design decisions:
    ///   - Implements Serializable ([Serializable]) for disk persistence across restarts.
    ///   - serialVersionUID equivalent provided for compatibility.
    ///   - Fields NOT readonly (despite immutability intent) because serialization requires it.
    ///   - Multiple constructors for: single-operand ops, binary ops, and error cases.
    ///   - Immutability enforced through constructor-only initialization (no setters).
    ///   - Thread-safe: once constructed the state never changes.
    ///
    /// Used by QuantityMeasurementCacheRepository to store operation history.
    /// </summary>
    [Serializable]
    public class QuantityMeasurementEntity
    {
        // ── Fields (no setters — immutable after construction) ─────────────────

        public string   OperationType  { get; private set; }
        public string?  FirstOperand   { get; private set; }
        public string?  SecondOperand  { get; private set; }
        public string?  Result         { get; private set; }
        public bool     IsError        { get; private set; }
        public string?  ErrorMessage   { get; private set; }
        public DateTime Timestamp      { get; private set; }

        // ── Constructor: single-operand operation (e.g. Conversion) ───────────

        /// <summary>
        /// For conversion: one input operand, one result operand.
        /// </summary>
        public QuantityMeasurementEntity(string operationType,
                                          QuantityDTO operand,
                                          QuantityDTO result)
        {
            OperationType = operationType;
            FirstOperand  = operand.ToString();
            SecondOperand = null;
            Result        = result.ToString();
            IsError       = false;
            ErrorMessage  = null;
            Timestamp     = DateTime.Now;
        }

        // ── Constructor: binary operation (e.g. Compare, Add, Subtract, Divide) ──

        /// <summary>
        /// For binary operations: two input operands, one string result.
        /// </summary>
        public QuantityMeasurementEntity(string operationType,
                                          QuantityDTO firstOperand,
                                          QuantityDTO secondOperand,
                                          string result)
        {
            OperationType = operationType;
            FirstOperand  = firstOperand.ToString();
            SecondOperand = secondOperand.ToString();
            Result        = result;
            IsError       = false;
            ErrorMessage  = null;
            Timestamp     = DateTime.Now;
        }

        // ── Constructor: error case ────────────────────────────────────────────

        /// <summary>
        /// For failed operations — captures error details.
        /// isError must be true to distinguish from a valid empty result.
        /// </summary>
        public QuantityMeasurementEntity(string operationType,
                                          QuantityDTO? firstOperand,
                                          QuantityDTO? secondOperand,
                                          string errorMessage,
                                          bool isError)
        {
            OperationType = operationType;
            FirstOperand  = firstOperand?.ToString();
            SecondOperand = secondOperand?.ToString();
            Result        = null;
            IsError       = isError;
            ErrorMessage  = errorMessage;
            Timestamp     = DateTime.Now;
        }

        // ── Display ────────────────────────────────────────────────────────────

        public override string ToString()
        {
            if (IsError)
                return $"[{Timestamp:HH:mm:ss}] {OperationType} | ERROR: {ErrorMessage}";

            string operands = SecondOperand != null
                ? $"{FirstOperand} | {SecondOperand}"
                : $"{FirstOperand}";

            return $"[{Timestamp:HH:mm:ss}] {OperationType} | {operands} => {Result}";
        }
    }
}
