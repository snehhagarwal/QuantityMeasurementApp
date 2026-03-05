using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// UC3–UC7: Data Access Layer for Length operations.
    /// Handles equality, tolerance, conversion, and addition for Length.
    /// Tolerance is provided in inches by the caller; internally converted to feet
    /// because FEET is the base unit for Length after the UC8 refactor.
    /// </summary>
    public class LengthRepository
    {
        /// <summary>UC3: Compares two Length objects for exact equality (base-unit comparison).</summary>
        public bool Compare(Length first, Length second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// UC4: Compares two Length objects within a tolerance supplied in inches.
        /// Converts both lengths and the tolerance to feet (base unit) before comparing.
        /// </summary>
        public bool CompareWithTolerance(Length first, Length second, double toleranceInInches)
        {
            if (toleranceInInches < 0)
                throw new ArgumentException("Tolerance cannot be negative");

            double toleranceInFeet = LengthUnit.INCHES.ConvertToBaseUnit(toleranceInInches);
            double difference = Math.Abs(first.ToBaseUnit() - second.ToBaseUnit());
            return difference <= toleranceInFeet;
        }

        /// <summary>UC5: Delegates conversion to the Length entity.</summary>
        public Length ConvertTo(Length length, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            return length.ConvertTo(targetUnit, decimalPlaces);
        }

        /// <summary>UC6: Adds two lengths; result in first operand's unit.</summary>
        public Length Add(Length first, Length second, int decimalPlaces = 2)
        {
            return first.Add(second, decimalPlaces);
        }

        /// <summary>UC7: Adds two lengths; result in the specified target unit.</summary>
        public Length Add(Length first, Length second, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            return Length.Add(first, second, targetUnit, decimalPlaces);
        }
    }
}