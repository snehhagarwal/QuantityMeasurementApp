namespace QuantityMeasurementModel.Dto
{
    /// <summary>
    /// UC15: Data Transfer Object for holding quantity measurement input/output data.
    /// Used as the standard contract between Controller and Service layers.
    ///
    /// Embedded enums (LengthUnit, WeightUnit, VolumeUnit, TemperatureUnit) allow
    /// callers to create QuantityDTO instances without importing internal entity types.
    /// </summary>
    public class QuantityDTO
    {
        // ── Embedded unit enums ────────────────────────────────────────────────

        public enum LengthUnit      { FEET = 1, INCHES, YARDS, CENTIMETERS }
        public enum WeightUnit      { KILOGRAM = 1, GRAM, POUND }
        public enum VolumeUnit      { LITRE = 1, MILLILITRE, GALLON }
        public enum TemperatureUnit { CELSIUS = 1, FAHRENHEIT, KELVIN }

        // ── Properties ─────────────────────────────────────────────────────────

        public double  Value           { get; set; }
        public string? Unit            { get; set; }
        public string? MeasurementType { get; set; }

        // ── Constructors ───────────────────────────────────────────────────────

        public QuantityDTO() { }

        public QuantityDTO(double value, string unit, string measurementType)
        {
            Value           = value;
            Unit            = unit?.ToUpper();
            MeasurementType = measurementType?.ToUpper();
        }

        public QuantityDTO(double value, LengthUnit unit)
            : this(value, unit.ToString(), "LENGTH") { }

        public QuantityDTO(double value, WeightUnit unit)
            : this(value, unit.ToString(), "WEIGHT") { }

        public QuantityDTO(double value, VolumeUnit unit)
            : this(value, unit.ToString(), "VOLUME") { }

        public QuantityDTO(double value, TemperatureUnit unit)
            : this(value, unit.ToString(), "TEMPERATURE") { }

        // ── Methods ────────────────────────────────────────────────────────────

        public override string ToString() => $"{Value:G} {Unit}";
    }
}
