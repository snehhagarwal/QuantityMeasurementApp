using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementModel.Units;
using QuantityMeasurementModel.Enums;
using QuantityMeasurementBusinessLayer.Unit;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC5: Unit-to-Unit Conversion Test Suite
    /// 
    /// Purpose:
    /// Test coverage for unit conversion functionality between different length units.
    /// Validates conversion accuracy, precision, error handling, and mathematical consistency.
    /// 
    /// Framework Used: MSTest
    /// </summary>
    [TestClass]
    public class LengthConversionTests
    {
        private const double EPSILON = 1e-6; // Small epsilon for floating-point comparisons (1e-6 as specified)

        /// <summary>
        /// Test: Convert 1.0 feet to inches
        /// Expected: 12.0 inches
        /// </summary>
        [TestMethod]
        public void TestConversion_FeetToInches()
        {
            double result = Length.Convert(1.0, LengthUnit.FEET, LengthUnit.INCHES);
            Assert.AreEqual(12.0, result, EPSILON, "convert(1.0, FEET, INCHES) should return 12.0");
        }

        /// <summary>
        /// Test: Convert 24.0 inches to feet
        /// Expected: 2.0 feet
        /// </summary>
        [TestMethod]
        public void TestConversion_InchesToFeet()
        {
            double result = Length.Convert(24.0, LengthUnit.INCHES, LengthUnit.FEET);
            Assert.AreEqual(2.0, result, EPSILON, "convert(24.0, INCHES, FEET) should return 2.0");
        }

        /// <summary>
        /// Test: Convert 1.0 yard to inches
        /// Expected: 36.0 inches
        /// </summary>
        [TestMethod]
        public void TestConversion_YardsToInches()
        {
            double result = Length.Convert(1.0, LengthUnit.YARDS, LengthUnit.INCHES);
            Assert.AreEqual(36.0, result, EPSILON, "convert(1.0, YARDS, INCHES) should return 36.0");
        }

        /// <summary>
        /// Test: Convert 72.0 inches to yards
        /// Expected: 2.0 yards
        /// </summary>
        [TestMethod]
        public void TestConversion_InchesToYards()
        {
            double result = Length.Convert(72.0, LengthUnit.INCHES, LengthUnit.YARDS);
            Assert.AreEqual(2.0, result, EPSILON, "convert(72.0, INCHES, YARDS) should return 2.0");
        }

        /// <summary>
        /// Test: Convert 2.54 centimeters to inches
        /// Expected: ~1.0 inches (within epsilon)
        /// </summary>
        [TestMethod]
        public void TestConversion_CentimetersToInches()
        {
            double result = Length.Convert(2.54, LengthUnit.CENTIMETERS, LengthUnit.INCHES);
            Assert.AreEqual(1.0, result, EPSILON, "convert(2.54, CENTIMETERS, INCHES) should return ~1.0 (within epsilon)");
        }

        /// <summary>
        /// Test: Convert 6.0 feet to yards
        /// Expected: 2.0 yards
        /// </summary>
        [TestMethod]
        public void TestConversion_FeetToYard()
        {
            double result = Length.Convert(6.0, LengthUnit.FEET, LengthUnit.YARDS);
            Assert.AreEqual(2.0, result, EPSILON, "convert(6.0, FEET, YARDS) should return 2.0");
        }

        /// <summary>
        /// Test: Round-trip conversion preserves value
        /// Given value v and units A, B: convert(convert(v, A, B), B, A) ≈ v within defined tolerance
        /// </summary>
        [TestMethod]
        public void TestConversion_RoundTrip_PreservesValue()
        {
            double originalValue = 10.0;
            LengthUnit unitA = LengthUnit.FEET;
            LengthUnit unitB = LengthUnit.INCHES;

            // Convert A → B
            double convertedAB = Length.Convert(originalValue, unitA, unitB);
            
            // Convert B → A
            double convertedBA = Length.Convert(convertedAB, unitB, unitA);

            // Round-trip should preserve original value within tolerance
            Assert.AreEqual(originalValue, convertedBA, EPSILON, 
                "Round-trip conversion should preserve original value within tolerance");
        }

        /// <summary>
        /// Test: Convert zero value
        /// Expected: 0.0
        /// </summary>
        [TestMethod]
        public void TestConversion_ZeroValue()
        {
            double result = Length.Convert(0.0, LengthUnit.FEET, LengthUnit.INCHES);
            Assert.AreEqual(0.0, result, EPSILON, "convert(0.0, FEET, INCHES) should return 0.0");
        }

        /// <summary>
        /// Test: Convert negative value
        /// Expected: -12.0 (behavior defined and asserted)
        /// </summary>
        [TestMethod]
        public void TestConversion_NegativeValue()
        {
            double result = Length.Convert(-1.0, LengthUnit.FEET, LengthUnit.INCHES);
            Assert.AreEqual(-12.0, result, EPSILON, "convert(-1.0, FEET, INCHES) should return -12.0");
        }

        /// <summary>
        /// Test: Invalid unit throws exception
        /// Passing a null or unsupported unit should throw an ArgumentException (or defined exception)
        /// </summary>
        [TestMethod]
        public void TestConversion_InvalidUnit_Throws()
        {
            // Test with UNKNOWN unit
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(10.0, LengthUnit.UNKNOWN, LengthUnit.INCHES);
            }, "Passing UNKNOWN unit should throw ArgumentException");

            // Test with UNKNOWN target unit
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(10.0, LengthUnit.FEET, LengthUnit.UNKNOWN);
            }, "Passing UNKNOWN target unit should throw ArgumentException");
        }

        /// <summary>
        /// Test: NaN or Infinite value throws exception
        /// Passing NaN or +/-Infinity as a value should result in a validation failure
        /// </summary>
        [TestMethod]
        public void TestConversion_NaNOrInfinite_Throws()
        {
            // Test NaN
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(double.NaN, LengthUnit.FEET, LengthUnit.INCHES);
            }, "Passing NaN should throw ArgumentException");

            // Test Positive Infinity
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(double.PositiveInfinity, LengthUnit.FEET, LengthUnit.INCHES);
            }, "Passing PositiveInfinity should throw ArgumentException");

            // Test Negative Infinity
            Assert.Throws<ArgumentException>(() =>
            {
                Length.Convert(double.NegativeInfinity, LengthUnit.FEET, LengthUnit.INCHES);
            }, "Passing NegativeInfinity should throw ArgumentException");
        }

        /// <summary>
        /// Test: Precision tolerance
        /// Conversion results are compared using a small epsilon (e.g., 1e-6) to account for floating-point rounding
        /// </summary>
        [TestMethod]
        public void TestConversion_PrecisionTolerance()
        {
            // Test conversion that may have floating-point precision issues
            // Use higher precision (6 decimal places) to get actual mathematical result without rounding
            double value = 1.0;
            double result = Length.Convert(value, LengthUnit.CENTIMETERS, LengthUnit.INCHES, 6);
            double expected = 0.393701; // 1 cm = 0.393701 inches

            // Compare using small epsilon (1e-6) to account for floating-point rounding
            Assert.AreEqual(expected, result, EPSILON, 
                "Conversion results should be compared using small epsilon (1e-6) to account for floating-point rounding");
        }
    }
}
