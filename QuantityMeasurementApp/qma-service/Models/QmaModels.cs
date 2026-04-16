using System.ComponentModel.DataAnnotations;

namespace QmaService.Models;

// ── Input DTOs ────────────────────────────────────────────────────────────────

public class QuantityInputDto
{
    [Required] public QuantityOperandDto ThisQuantityDTO { get; set; } = new();
    [Required] public QuantityOperandDto ThatQuantityDTO { get; set; } = new();
}

public class QuantityOperandDto : IValidatableObject
{
    [Required] public double Value { get; set; }

    [Required]
    [RegularExpression(@"^(FEET|INCHES|YARDS|CENTIMETERS|KILOGRAM|GRAM|POUND|LITRE|MILLILITRE|GALLON|CELSIUS|FAHRENHEIT|KELVIN)$",
        ErrorMessage = "Unit must be a valid unit.")]
    public string Unit { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(LENGTH|WEIGHT|VOLUME|TEMPERATURE)$",
        ErrorMessage = "measurementType must be LENGTH, WEIGHT, VOLUME, or TEMPERATURE.")]
    public string MeasurementType { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        bool ok = MeasurementType?.ToUpper() switch
        {
            "LENGTH"      => IsIn(Unit, "FEET", "INCHES", "YARDS", "CENTIMETERS"),
            "WEIGHT"      => IsIn(Unit, "KILOGRAM", "GRAM", "POUND"),
            "VOLUME"      => IsIn(Unit, "LITRE", "MILLILITRE", "GALLON"),
            "TEMPERATURE" => IsIn(Unit, "CELSIUS", "FAHRENHEIT", "KELVIN"),
            _             => false
        };
        if (!ok)
            yield return new ValidationResult(
                $"Unit '{Unit}' is not valid for measurementType '{MeasurementType}'.",
                new[] { nameof(Unit) });
    }

    private static bool IsIn(string? v, params string[] allowed) =>
        v != null && Array.Exists(allowed, a => a == v.ToUpper());
}

// ── Response DTO ──────────────────────────────────────────────────────────────

public class QuantityMeasurementDto
{
    public long   Id             { get; set; }
    public string OperationType  { get; set; } = string.Empty;
    public string FormattedResult { get; set; } = string.Empty;
    public double? ResultValue   { get; set; }
    public string? ResultUnit    { get; set; }
    public string? ResultMeasurementType { get; set; }
    public bool   IsSuccessful   { get; set; }
    public string? ErrorDetails  { get; set; }
    public long?  UserId         { get; set; }
    public DateTime CreatedAt    { get; set; }

    // Operand details
    public double? FirstOperandValue    { get; set; }
    public string? FirstOperandUnit     { get; set; }
    public string? FirstOperandCategory { get; set; }
    public double? SecondOperandValue   { get; set; }
    public string? SecondOperandUnit    { get; set; }
    public string? SecondOperandCategory { get; set; }
    public string? TargetUnit           { get; set; }
}
