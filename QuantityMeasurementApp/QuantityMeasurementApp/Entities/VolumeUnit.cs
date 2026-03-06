using System;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC11: Enum representing supported volume units.
    /// Conversion factors are defined relative to the base unit LITRE.
    ///
    /// Conversion factors:
    ///   LITRE      : 1.0       (base unit)
    ///   MILLILITRE : 0.001     (1 mL = 0.001 L)
    ///   GALLON     : 3.78541   (1 US gallon ≈ 3.78541 L)
    ///
    /// Implements IMeasurable so it can be used directly with the generic
    /// Quantity&lt;TUnit&gt; class without any changes to that class.
    /// </summary>
    public enum VolumeUnit
    {
        UNKNOWN = 0,
        LITRE,
        MILLILITRE,
        GALLON
    }
}