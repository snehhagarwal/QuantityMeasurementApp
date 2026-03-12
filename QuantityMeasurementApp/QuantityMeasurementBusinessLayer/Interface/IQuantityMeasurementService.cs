using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>
    /// UC15: Single service interface for all quantity measurement operations.
    ///
    /// Accepts QuantityDTO input and returns QuantityDTO output — fully decoupled
    /// from internal Quantity&lt;TUnit&gt; representation.
    ///
    /// Replaces the six separate service interfaces (ILengthService, IWeightService,
    /// IVolumeService, ITemperatureService, IFeetService, IInchesService) with one
    /// unified contract following the Single Responsibility Principle.
    ///
    /// Supported operations:
    ///   Compare  — equality check between two quantities
    ///   Convert  — unit conversion
    ///   Add      — addition of two quantities
    ///   Subtract — subtraction of two quantities
    ///   Divide   — dimensionless ratio of two quantities
    /// </summary>
    public interface IQuantityMeasurementService
    {
        /// <summary>
        /// Compares two quantities for equality.
        /// Returns QuantityDTO with Unit = "RESULT" and Value = 1 (true) or 0 (false).
        /// MeasurementType contains the string "true" or "false" for readability.
        /// </summary>
        QuantityDTO Compare(QuantityDTO first, QuantityDTO second);

        /// <summary>
        /// Converts a quantity to the target unit specified by targetUnit.
        /// Returns QuantityDTO representing the converted value in targetUnit's unit.
        /// </summary>
        QuantityDTO Convert(QuantityDTO quantity, QuantityDTO targetUnit);

        /// <summary>
        /// Adds two quantities. Result is expressed in the first operand's unit.
        /// Throws QuantityMeasurementException for cross-category or unsupported ops.
        /// </summary>
        QuantityDTO Add(QuantityDTO first, QuantityDTO second);

        /// <summary>
        /// Subtracts second from first. Result is expressed in the first operand's unit.
        /// Throws QuantityMeasurementException for cross-category or unsupported ops.
        /// </summary>
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);

        /// <summary>
        /// Divides first by second. Returns a dimensionless ratio as QuantityDTO.
        /// Unit = "RATIO", MeasurementType = "DIMENSIONLESS".
        /// Throws QuantityMeasurementException for cross-category or unsupported ops.
        /// </summary>
        QuantityDTO Divide(QuantityDTO first, QuantityDTO second);
    }
}
