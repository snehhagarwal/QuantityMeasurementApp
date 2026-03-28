using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC17 Step 3: JPA @Entity equivalent — maps to quantity_measurements table.
    ///
    /// All fields from the spec Step 3 are present:
    ///   id, operationType, firstOperandValue/Unit/Category/Display,
    ///   secondOperandValue/Unit/Category/Display, targetUnit,
    ///   resultValue/Unit/MeasurementType/FormattedResult,
    ///   isSuccessful, errorDetails, createdAt, updatedAt.
    /// </summary>
    [Table("quantity_measurements")]
    public class QuantityMeasurement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("operation_type")]
        public string OperationType { get; set; } = string.Empty;

        // ── First operand ─────────────────────────────────────────────────────
        [Column("first_operand_value")]
        public double? FirstOperandValue { get; set; }

        [MaxLength(30)]
        [Column("first_operand_unit")]
        public string? FirstOperandUnit { get; set; }

        [MaxLength(20)]
        [Column("first_operand_category")]
        public string? FirstOperandCategory { get; set; }

        [MaxLength(100)]
        [Column("first_operand_display")]
        public string? FirstOperandDisplay { get; set; }

        // ── Second operand ────────────────────────────────────────────────────
        [Column("second_operand_value")]
        public double? SecondOperandValue { get; set; }

        [MaxLength(30)]
        [Column("second_operand_unit")]
        public string? SecondOperandUnit { get; set; }

        [MaxLength(20)]
        [Column("second_operand_category")]
        public string? SecondOperandCategory { get; set; }

        [MaxLength(100)]
        [Column("second_operand_display")]
        public string? SecondOperandDisplay { get; set; }

        // ── Target unit (CONVERT) ─────────────────────────────────────────────
        [MaxLength(30)]
        [Column("target_unit")]
        public string? TargetUnit { get; set; }

        // ── Result ────────────────────────────────────────────────────────────
        [Column("result_value")]
        public double? ResultValue { get; set; }

        [MaxLength(30)]
        [Column("result_unit")]
        public string? ResultUnit { get; set; }

        [MaxLength(20)]
        [Column("result_measurement_type")]
        public string? ResultMeasurementType { get; set; }

        [MaxLength(200)]
        [Column("formatted_result")]
        public string? FormattedResult { get; set; }

        // ── Status ────────────────────────────────────────────────────────────
        [Column("is_successful")]
        public bool IsSuccessful { get; set; }

        [MaxLength(500)]
        [Column("error_details")]
        public string? ErrorDetails { get; set; }

        // ── Timestamps ────────────────────────────────────────────────────────
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public QuantityMeasurement()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public bool IsError => !IsSuccessful;
    }
}
