using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// UC2: Data Access Layer for Inches comparisons.
    /// Responsible for exact and tolerance-based Inches equality checks.
    /// </summary>
    public class InchesRepository
    {
        /// <summary>Compares two Inches objects for exact equality.</summary>
        public bool CompareInches(Inches firstMeasurement, Inches secondMeasurement)
        {
            return firstMeasurement.Equals(secondMeasurement);
        }

        /// <summary>
        /// Compares two Inches objects within a tolerance.
        /// Throws ArgumentException if tolerance is negative.
        /// </summary>
        public bool CompareWithTolerance(Inches firstMeasurement, Inches secondMeasurement, double tolerance)
        {
            if (tolerance < 0)
                throw new ArgumentException("Tolerance cannot be negative");

            double difference = Math.Abs(firstMeasurement.Value - secondMeasurement.Value);
            return difference <= tolerance;
        }
    }
}