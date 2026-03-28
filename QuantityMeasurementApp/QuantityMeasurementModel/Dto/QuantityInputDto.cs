using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// UC17 Step 7A: Request body DTO.
    /// Field names exactly match the spec curl examples:
    ///   thisQuantityDTO and thatQuantityDTO.
    /// </summary>
    public class QuantityInputDto
    {
        [Required(ErrorMessage = "thisQuantityDTO is required.")]
        public QuantityOperandDto ThisQuantityDTO { get; set; } = new();

        [Required(ErrorMessage = "thatQuantityDTO is required.")]
        public QuantityOperandDto ThatQuantityDTO { get; set; } = new();
    }

    /// <summary>
    /// UC17 Step 5B: Single operand DTO with validation.
    /// @NotNull → [Required], @Pattern → [RegularExpression], @AssertTrue → IValidatableObject
    /// </summary>
    public class QuantityOperandDto : IValidatableObject
    {
        [Required(ErrorMessage = "value is required.")]
        public double Value { get; set; }

        [Required(ErrorMessage = "unit is required.")]
        [RegularExpression(
            @"^(FEET|INCHES|YARDS|CENTIMETERS|KILOGRAM|GRAM|POUND|LITRE|MILLILITRE|GALLON|CELSIUS|FAHRENHEIT|KELVIN)$",
            ErrorMessage = "Unit must be valid for the specified measurement type")]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "measurementType is required.")]
        [RegularExpression(
            @"^(LENGTH|WEIGHT|VOLUME|TEMPERATURE)$",
            ErrorMessage = "measurementType must be LENGTH, WEIGHT, VOLUME, or TEMPERATURE.")]
        public string MeasurementType { get; set; } = string.Empty;

        /// <summary>
        /// @AssertTrue equivalent: cross-field validation ensuring unit
        /// belongs to the declared measurementType.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            bool ok = MeasurementType?.ToUpper() switch
            {
                "LENGTH"      => IsIn(Unit, "FEET","INCHES","YARDS","CENTIMETERS"),
                "WEIGHT"      => IsIn(Unit, "KILOGRAM","GRAM","POUND"),
                "VOLUME"      => IsIn(Unit, "LITRE","MILLILITRE","GALLON"),
                "TEMPERATURE" => IsIn(Unit, "CELSIUS","FAHRENHEIT","KELVIN"),
                _             => false
            };
            if (!ok)
                yield return new ValidationResult(
                    $"Unit '{Unit}' is not valid for measurementType '{MeasurementType}'.",
                    new[] { nameof(Unit) });
        }

        private static bool IsIn(string? v, params string[] allowed) =>
            v != null && System.Array.Exists(allowed, a => a == v.ToUpper());
    }
}
