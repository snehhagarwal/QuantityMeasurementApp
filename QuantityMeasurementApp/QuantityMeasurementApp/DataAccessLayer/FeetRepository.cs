using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// Data Access Layer.
    /// Responsible for comparing Feet objects.
    /// </summary>
    public class FeetRepository
    {
        /// <summary>
        /// Compare two Feet objects for exact equality.
        /// </summary>
        public bool CompareFeet(Feet firstMeasurement, Feet secondMeasurement)
        {
            return firstMeasurement.Equals(secondMeasurement);
        }

        /// <summary>
        /// Compare two Feet objects using tolerance.
        /// Unique Feature.
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