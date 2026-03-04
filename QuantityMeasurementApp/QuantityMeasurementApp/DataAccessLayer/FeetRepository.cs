using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// UC1: Data Access Layer for Feet comparisons.
    /// Responsible for exact and tolerance-based Feet equality checks.
    /// </summary>
    public class FeetRepository
    {
        /// <summary>Compares two Feet objects for exact equality.</summary>
        public bool CompareFeet(Feet firstMeasurement, Feet secondMeasurement)
        {
            return firstMeasurement.Equals(secondMeasurement);
        }

        /// <summary>
        /// Compares two Feet objects within a tolerance.
        /// Throws ArgumentException if tolerance is negative.
        /// </summary>
        public bool CompareFeetWithTolerance(Feet firstMeasurement, Feet secondMeasurement, double tolerance)
        {
            if (tolerance < 0)
                throw new ArgumentException("Tolerance cannot be negative");

            double difference = Math.Abs(firstMeasurement.Value - secondMeasurement.Value);
            return difference <= tolerance;
        }
    }
}