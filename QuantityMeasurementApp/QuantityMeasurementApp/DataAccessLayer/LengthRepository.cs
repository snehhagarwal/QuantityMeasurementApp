using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// Repository class for Length comparison.
    /// </summary>
    public class LengthRepository
    {
        /// <summary>
        /// Compare two Length objects.
        /// </summary>
        public bool Compare(Length first, Length second)
        {
            return first.Equals(second);
        }

        public bool CompareWithTolerance(Length first,Length second,double tolerance)
        {
            if (tolerance < 0)
                throw new ArgumentException("Tolerance cannot be negative");

            // UC3/UC4 UI and existing flows provide tolerance in INCHES.
            // UC8 refactor uses FEET as base unit internally, so convert tolerance inches -> feet.
            double toleranceInFeet = LengthUnit.INCHES.ConvertToBaseUnit(tolerance);

            double difference = Math.Abs(first.ToBaseUnit() - second.ToBaseUnit());
            return difference <= toleranceInFeet;
        }
    }
}