using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementBusinessLayer.Interface
{
    /// <summary>UC15 console / domain service contract (QuantityDTO-based).</summary>
    public interface IQuantityMeasurementService
    {
        QuantityDTO Compare(QuantityDTO first, QuantityDTO second);
        QuantityDTO Convert(QuantityDTO quantity, QuantityDTO targetUnit);
        QuantityDTO Add(QuantityDTO first, QuantityDTO second);
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);
        QuantityDTO Divide(QuantityDTO first, QuantityDTO second);
    }
}
