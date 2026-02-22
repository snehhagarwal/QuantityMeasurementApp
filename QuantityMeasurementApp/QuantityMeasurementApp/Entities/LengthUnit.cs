namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// Enum representing supported length units.
    /// Conversion factor is defined relative to base unit Inches.
    /// </summary>
    public enum LengthUnit
    {
        /// <summary>
        /// Feet unit.
        /// 1 Foot = 12 Inches
        /// </summary>
        FEET = 12,

        /// <summary>
        /// Inches unit.
        /// Base unit.
        /// </summary>
        INCHES = 1
    }
}