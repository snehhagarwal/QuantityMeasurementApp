using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC11: Service interface for Volume operations.
    /// Covers equality, conversion, and addition
    /// for all supported volume units (LITRE, MILLILITRE, GALLON).
    ///
    /// UC12: Extended with Subtract and Divide operations.
    /// Implemented by VolumeService; registered in DI container for ASP.NET migration.
    /// </summary>
    public interface IVolumeService
    {
        /// <summary>Returns true if both Volume quantities represent the same physical volume.</summary>
        bool AreEqual(Quantity<VolumeUnitMeasurable> first, Quantity<VolumeUnitMeasurable> second);

        /// <summary>Converts a Volume quantity to the specified target unit.</summary>
        Quantity<VolumeUnitMeasurable> ConvertTo(Quantity<VolumeUnitMeasurable> volume,
                                                  VolumeUnitMeasurable targetUnit,
                                                  int decimalPlaces = 2);

        /// <summary>Adds two Volume quantities; result in first operand's unit.</summary>
        Quantity<VolumeUnitMeasurable> Add(Quantity<VolumeUnitMeasurable> first,
                                           Quantity<VolumeUnitMeasurable> second,
                                           int decimalPlaces = 2);

        /// <summary>Adds two Volume quantities; result in the specified target unit.</summary>
        Quantity<VolumeUnitMeasurable> Add(Quantity<VolumeUnitMeasurable> first,
                                           Quantity<VolumeUnitMeasurable> second,
                                           VolumeUnitMeasurable targetUnit,
                                           int decimalPlaces = 2);

        // UC12 ─────────────────────────────────────────────────────────────────

        /// <summary>Subtracts second from first; result in first operand's unit.</summary>
        Quantity<VolumeUnitMeasurable> Subtract(Quantity<VolumeUnitMeasurable> first,
                                                 Quantity<VolumeUnitMeasurable> second,
                                                 int decimalPlaces = 2);

        /// <summary>Subtracts second from first; result in the specified target unit.</summary>
        Quantity<VolumeUnitMeasurable> Subtract(Quantity<VolumeUnitMeasurable> first,
                                                 Quantity<VolumeUnitMeasurable> second,
                                                 VolumeUnitMeasurable targetUnit,
                                                 int decimalPlaces = 2);

        /// <summary>Divides first by second and returns the dimensionless ratio.</summary>
        double Divide(Quantity<VolumeUnitMeasurable> first, Quantity<VolumeUnitMeasurable> second);
    }
}