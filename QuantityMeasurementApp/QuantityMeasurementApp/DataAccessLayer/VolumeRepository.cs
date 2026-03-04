using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// UC11: Data Access Layer for Volume operations.
    /// Handles equality, conversion, and addition for Quantity&lt;VolumeUnitMeasurable&gt;.
    /// Delegates all domain logic to the Quantity&lt;TUnit&gt; entity — keeping the DAL thin.
    /// </summary>
    public class VolumeRepository
    {
        /// <summary>Compares two Volume quantities for exact equality (base-unit comparison).</summary>
        public bool Compare(Quantity<VolumeUnitMeasurable> first, Quantity<VolumeUnitMeasurable> second)
        {
            return first.Equals(second);
        }

        /// <summary>Converts a Volume quantity to the specified target unit.</summary>
        public Quantity<VolumeUnitMeasurable> ConvertTo(Quantity<VolumeUnitMeasurable> volume,
                                                         VolumeUnitMeasurable targetUnit,
                                                         int decimalPlaces = 2)
        {
            return volume.ConvertTo(targetUnit, decimalPlaces);
        }

        /// <summary>Adds two Volume quantities; result in first operand's unit.</summary>
        public Quantity<VolumeUnitMeasurable> Add(Quantity<VolumeUnitMeasurable> first,
                                                   Quantity<VolumeUnitMeasurable> second,
                                                   int decimalPlaces = 2)
        {
            return first.Add(second, decimalPlaces);
        }

        /// <summary>Adds two Volume quantities; result in the specified target unit.</summary>
        public Quantity<VolumeUnitMeasurable> Add(Quantity<VolumeUnitMeasurable> first,
                                                   Quantity<VolumeUnitMeasurable> second,
                                                   VolumeUnitMeasurable targetUnit,
                                                   int decimalPlaces = 2)
        {
            return first.Add(second, targetUnit, decimalPlaces);
        }
    }
}