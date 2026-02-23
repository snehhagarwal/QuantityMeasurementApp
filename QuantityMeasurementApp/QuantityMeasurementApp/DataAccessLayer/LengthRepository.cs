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

            double difference =Math.Abs(first.ToBaseUnit() -second.ToBaseUnit());
            return difference <= tolerance;
        }
    }
}