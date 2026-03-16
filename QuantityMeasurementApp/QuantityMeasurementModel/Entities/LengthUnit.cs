namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Enum representing supported length units.
    /// Conversion factor is defined relative to base unit Inches.
    /// </summary>
    public enum LengthUnit
    {
        UNKNOWN=0,
        /// <summary>
        /// Feet unit.
        /// 1 Foot = 12 Inches
        /// </summary>
        FEET,
        /// <summary>
        /// Inches unit.
        /// Base unit.
        /// </summary>
        INCHES,
        YARDS,
        CENTIMETERS
    }
}