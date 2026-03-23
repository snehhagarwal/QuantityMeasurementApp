using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// UC17 Step 5C: Response DTO — field names exactly match the spec curl response:
    ///   thisValue, thisUnit, thisMeasurementType,
    ///   thatValue, thatUnit, thatMeasurementType,
    ///   operation, resultString, resultValue, resultUnit, resultMeasurementType,
    ///   errorMessage, error
    ///
    /// Static factory methods mirror Java:
    ///   fromEntity()     — QuantityMeasurement -&gt; QuantityMeasurementDto
    ///   fromEntityList() — List&lt;entity&gt; -&gt; List&lt;DTO&gt;
    ///   toEntity()       — DTO -&gt; entity
    /// </summary>
    public class QuantityMeasurementDto
    {
        // ── First operand (thisQuantityDTO) ───────────────────────────────────
        public double?  ThisValue           { get; set; }
        public string?  ThisUnit            { get; set; }
        public string?  ThisMeasurementType { get; set; }

        // ── Second operand (thatQuantityDTO) ──────────────────────────────────
        public double?  ThatValue           { get; set; }
        public string?  ThatUnit            { get; set; }
        public string?  ThatMeasurementType { get; set; }

        // ── Operation ─────────────────────────────────────────────────────────
        public string?  Operation           { get; set; }

        // ── Result ────────────────────────────────────────────────────────────
        public string?  ResultString        { get; set; }
        public double   ResultValue         { get; set; }
        public string?  ResultUnit          { get; set; }
        public string?  ResultMeasurementType { get; set; }

        // ── Error / audit ─────────────────────────────────────────────────────
        public string?  ErrorMessage        { get; set; }
        public bool     Error               { get; set; }

        // ── Aliases for test compatibility ────────────────────────────────────
        /// <summary>Alias for Error — used by tests expecting IsError.</summary>
        public bool IsError
        {
            get => Error;
            set => Error = value;
        }

        /// <summary>Alias for ResultMeasurementType — used by tests expecting MeasurementType.</summary>
        public string? MeasurementType
        {
            get => ResultMeasurementType;
            set => ResultMeasurementType = value;
        }

        // ── Metadata ─────────────────────────────────────────────────────────
        public long?    Id                  { get; set; }
        public DateTime? Timestamp          { get; set; }

        // ── Static factory methods ────────────────────────────────────────────

        public static QuantityMeasurementDto FromEntity(QuantityMeasurement e) =>
            new()
            {
                Id                  = e.Id,
                Operation           = e.OperationType,
                ThisValue           = e.FirstOperandValue,
                ThisUnit            = e.FirstOperandUnit,
                ThisMeasurementType = e.FirstOperandCategory,
                ThatValue           = e.SecondOperandValue,
                ThatUnit            = e.SecondOperandUnit,
                ThatMeasurementType = e.SecondOperandCategory,
                ResultString        = e.FormattedResult,
                ResultValue         = e.ResultValue ?? 0.0,
                ResultUnit          = e.ResultUnit,
                ResultMeasurementType = e.ResultMeasurementType,
                ErrorMessage        = e.ErrorDetails,
                Error               = !e.IsSuccessful,
                Timestamp           = e.CreatedAt
            };

        public static List<QuantityMeasurementDto> FromEntityList(
            IEnumerable<QuantityMeasurement> entities) =>
            entities.Select(FromEntity).ToList();

        public QuantityMeasurement ToEntity() =>
            new()
            {
                OperationType       = Operation    ?? "UNKNOWN",
                FirstOperandValue   = ThisValue,
                FirstOperandUnit    = ThisUnit,
                FirstOperandCategory = ThisMeasurementType,
                FirstOperandDisplay = ThisValue.HasValue ? $"{ThisValue} {ThisUnit}" : null,
                SecondOperandValue  = ThatValue,
                SecondOperandUnit   = ThatUnit,
                SecondOperandCategory = ThatMeasurementType,
                SecondOperandDisplay = ThatValue.HasValue ? $"{ThatValue} {ThatUnit}" : null,
                FormattedResult     = ResultString,
                ResultValue         = ResultValue,
                ResultUnit          = ResultUnit,
                ResultMeasurementType = ResultMeasurementType,
                IsSuccessful        = !Error,
                ErrorDetails        = ErrorMessage,
                CreatedAt           = Timestamp ?? DateTime.UtcNow,
                UpdatedAt           = DateTime.UtcNow
            };
    }
}
