using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// UC9: Data Access Layer for Weight operations.
    /// Handles equality, conversion, and addition for Weight.
    /// Delegates all domain logic to the Weight entity — keeping the DAL thin.
    /// </summary>
    public class WeightRepository
    {
        /// <summary>Compares two Weight objects for exact equality (base-unit comparison).</summary>
        public bool Compare(Weight first, Weight second)
        {
            return first.Equals(second);
        }

        /// <summary>Converts a Weight to the specified target unit.</summary>
        public Weight ConvertTo(Weight weight, WeightUnit targetUnit, int decimalPlaces = 2)
        {
            return weight.ConvertTo(targetUnit, decimalPlaces);
        }

        /// <summary>Adds two Weight values; result in first operand's unit.</summary>
        public Weight Add(Weight first, Weight second, int decimalPlaces = 2)
        {
            return first.Add(second, decimalPlaces);
        }

        /// <summary>Adds two Weight values; result in the specified target unit.</summary>
        public Weight Add(Weight first, Weight second, WeightUnit targetUnit, int decimalPlaces = 2)
        {
            return Weight.Add(first, second, targetUnit, decimalPlaces);
        }
    }
}