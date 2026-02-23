using QuantityMeasurementApp.Entities;
using System;

namespace QuantityMeasurementApp.DataAccessLayer
{
    /// <summary>
    /// Handles Inches comparison.
    /// </summary>
    public class InchesRepository
    {
        public bool CompareInches(Inches firstMeasurement, Inches secondMeasurement)
        {
            return firstMeasurement.Equals(secondMeasurement);
        }

        public bool CompareWithTolerance(Inches firstMeasurement,Inches secondMeasurement,double tolerance)
        {
            if (tolerance < 0)
                throw new ArgumentException("Tolerance cannot be negative");

            double difference =
                Math.Abs(firstMeasurement.Value - secondMeasurement.Value);

            return difference <= tolerance;
        }
    }
}