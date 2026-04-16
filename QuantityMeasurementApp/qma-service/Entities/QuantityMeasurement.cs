using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QmaService.Entities;

[Table("quantity_measurements")]
public class QuantityMeasurement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Required][MaxLength(20)][Column("operation_type")]
    public string OperationType { get; set; } = string.Empty;

    [Column("first_operand_value")]  public double? FirstOperandValue    { get; set; }
    [MaxLength(30)][Column("first_operand_unit")]     public string? FirstOperandUnit     { get; set; }
    [MaxLength(20)][Column("first_operand_category")] public string? FirstOperandCategory { get; set; }
    [MaxLength(100)][Column("first_operand_display")] public string? FirstOperandDisplay  { get; set; }

    [Column("second_operand_value")]  public double? SecondOperandValue    { get; set; }
    [MaxLength(30)][Column("second_operand_unit")]     public string? SecondOperandUnit     { get; set; }
    [MaxLength(20)][Column("second_operand_category")] public string? SecondOperandCategory { get; set; }
    [MaxLength(100)][Column("second_operand_display")] public string? SecondOperandDisplay  { get; set; }

    [MaxLength(30)][Column("target_unit")] public string? TargetUnit { get; set; }

    [Column("result_value")]           public double? ResultValue          { get; set; }
    [MaxLength(30)][Column("result_unit")]             public string? ResultUnit            { get; set; }
    [MaxLength(20)][Column("result_measurement_type")] public string? ResultMeasurementType { get; set; }
    [MaxLength(200)][Column("formatted_result")]       public string? FormattedResult       { get; set; }

    [Column("is_successful")]   public bool IsSuccessful { get; set; }
    [MaxLength(500)][Column("error_details")] public string? ErrorDetails { get; set; }
    [Column("user_id")]         public long? UserId { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
