using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC7: Tests for addition with explicit target unit.
    /// </summary>
    [TestClass]
    public class LengthAdditionTargetUnitTests
    {
        private const double EPSILON = 0.01;

        // testAddition_ExplicitTargetUnit_Feet
        // Add (Quantity(1.0, FEET), Quantity(12.0, INCHES), FEET) should return Quantity(2.0, FEET).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Feet()
        {
            Length firstLength = new Length(1.0, LengthUnit.FEET);
            Length secondLength = new Length(12.0, LengthUnit.INCHES);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.FEET);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_Inches
        // Add (Quantity(1.0, FEET), Quantity(12.0, INCHES), INCHES) should return Quantity(24.0, INCHES).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Inches()
        {
            Length firstLength = new Length(1.0, LengthUnit.FEET);
            Length secondLength = new Length(12.0, LengthUnit.INCHES);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.INCHES);

            Assert.AreEqual(24.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_Yards
        // Add (Quantity(1.0, FEET), Quantity(12.0, INCHES), YARDS) should return Quantity(~0.667, YARDS).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Yards()
        {
            Length firstLength = new Length(1.0, LengthUnit.FEET);
            Length secondLength = new Length(12.0, LengthUnit.INCHES);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.YARDS);

            double expected = 2.0 / 3.0; // ≈ 0.6667 yards
            Assert.AreEqual(expected, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_Centimeters
        // Add (Quantity(1.0, INCHES), Quantity(1.0, INCHES), CENTIMETERS) should return Quantity(~5.08, CENTIMETERS).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Centimeters()
        {
            Length firstLength = new Length(1.0, LengthUnit.INCHES);
            Length secondLength = new Length(1.0, LengthUnit.INCHES);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.CENTIMETERS);

            double expected = 5.08; // 2 inches in centimeters
            Assert.AreEqual(expected, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.CENTIMETERS, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_SameAsFirstOperand
        // Add (Quantity(2.0, YARDS), Quantity(3.0, FEET), YARDS) should return Quantity(3.0, YARDS).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsFirstOperand()
        {
            Length firstLength = new Length(2.0, LengthUnit.YARDS);
            Length secondLength = new Length(3.0, LengthUnit.FEET);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.YARDS);

            Assert.AreEqual(3.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_SameAsSecondOperand
        // Add (Quantity(2.0, YARDS), Quantity(3.0, FEET), FEET) should return Quantity(9.0, FEET).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SameAsSecondOperand()
        {
            Length firstLength = new Length(2.0, LengthUnit.YARDS);
            Length secondLength = new Length(3.0, LengthUnit.FEET);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.FEET);

            Assert.AreEqual(9.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_Commutativity
        // Add (1.0 FEET, 12.0 INCHES, YARDS) should equal add(12.0 INCHES, 1.0 FEET, YARDS).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Commutativity()
        {
            Length firstLength = new Length(1.0, LengthUnit.FEET);
            Length secondLength = new Length(12.0, LengthUnit.INCHES);

            Length sumFirstThenSecond = Length.Add(firstLength, secondLength, LengthUnit.YARDS);
            Length sumSecondThenFirst = Length.Add(secondLength, firstLength, LengthUnit.YARDS);

            Assert.AreEqual(sumFirstThenSecond.Value, sumSecondThenFirst.Value, EPSILON);
            Assert.AreEqual(sumFirstThenSecond.Unit, sumSecondThenFirst.Unit);
        }

        // testAddition_ExplicitTargetUnit_WithZero
        // Add (5.0 FEET, 0.0 INCHES, YARDS) should return Quantity(~1.667, YARDS).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_WithZero()
        {
            Length firstLength = new Length(5.0, LengthUnit.FEET);
            Length secondLength = new Length(0.0, LengthUnit.INCHES);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.YARDS);

            double expected = 5.0 * 12.0 / 36.0; // 5 feet to yards
            Assert.AreEqual(expected, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_NegativeValues
        // Add (5.0 FEET, -2.0 FEET, INCHES) should return Quantity(36.0, INCHES).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NegativeValues()
        {
            Length firstLength = new Length(5.0, LengthUnit.FEET);
            Length secondLength = new Length(-2.0, LengthUnit.FEET);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.INCHES);

            Assert.AreEqual(36.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_NullTargetUnit
        // In C#, LengthUnit is an enum (value type) and cannot be null.
        // We treat UNKNOWN as an invalid "null-like" target and expect an exception.
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_NullTargetUnit()
        {
            Length firstLength = new Length(1.0, LengthUnit.FEET);
            Length secondLength = new Length(12.0, LengthUnit.INCHES);

            Assert.Throws<ArgumentException>(() =>
            {
                Length.Add(firstLength, secondLength, LengthUnit.UNKNOWN);
            });
        }

        // testAddition_ExplicitTargetUnit_LargeToSmallScale
        // Add (1000.0 FEET, 500.0 FEET, INCHES) should return Quantity(18000.0, INCHES).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_LargeToSmallScale()
        {
            Length firstLength = new Length(1000.0, LengthUnit.FEET);
            Length secondLength = new Length(500.0, LengthUnit.FEET);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.INCHES);

            Assert.AreEqual(18000.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_SmallToLargeScale
        // Add (12.0 INCHES, 12.0 INCHES, YARDS) should return Quantity(~0.667, YARDS).
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_SmallToLargeScale()
        {
            Length firstLength = new Length(12.0, LengthUnit.INCHES);
            Length secondLength = new Length(12.0, LengthUnit.INCHES);

            Length result = Length.Add(firstLength, secondLength, LengthUnit.YARDS);

            double expected = 24.0 / 36.0; // 24 inches to yards
            Assert.AreEqual(expected, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.Unit);
        }

        // testAddition_ExplicitTargetUnit_AllUnitCombinations
        // Comprehensive test that checks all unit pairs with multiple target units.
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_AllUnitCombinations()
        {
            LengthUnit[] units =
            {
                LengthUnit.FEET,
                LengthUnit.INCHES,
                LengthUnit.YARDS,
                LengthUnit.CENTIMETERS
            };

            double firstValue = 3.5;
            double secondValue = 1.25;

            foreach (LengthUnit firstUnit in units)
            {
                foreach (LengthUnit secondUnit in units)
                {
                    foreach (LengthUnit targetUnit in units)
                    {
                        Length firstLength = new Length(firstValue, firstUnit);
                        Length secondLength = new Length(secondValue, secondUnit);

                        Length result = Length.Add(firstLength, secondLength, targetUnit);

                        // Expected value computed by converting both to target, adding, then rounding.
                        double firstInTarget = Length.Convert(firstValue, firstUnit, targetUnit, 6);
                        double secondInTarget = Length.Convert(secondValue, secondUnit, targetUnit, 6);
                        double expected = Math.Round(firstInTarget + secondInTarget, 2);

                        Assert.AreEqual(expected, result.Value, EPSILON,
                            $"Mismatch for {firstUnit} + {secondUnit} in {targetUnit}");
                        Assert.AreEqual(targetUnit, result.Unit);
                    }
                }
            }
        }

        // testAddition_ExplicitTargetUnit_PrecisionTolerance
        // Multiple additions with explicit target units, checked with a small epsilon.
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_PrecisionTolerance()
        {
            var testCases = new[]
            {
                new { FirstValue = 1.0, FirstUnit = LengthUnit.FEET, SecondValue = 12.0, SecondUnit = LengthUnit.INCHES, Target = LengthUnit.YARDS },
                new { FirstValue = 2.54, FirstUnit = LengthUnit.CENTIMETERS, SecondValue = 1.0, SecondUnit = LengthUnit.INCHES, Target = LengthUnit.CENTIMETERS },
                new { FirstValue = 0.5, FirstUnit = LengthUnit.YARDS, SecondValue = 18.0, SecondUnit = LengthUnit.INCHES, Target = LengthUnit.FEET }
            };

            // Use the same epsilon as other tests, since the add method
            // rounds to 2 decimal places internally.
            const double smallEpsilon = EPSILON;

            foreach (var test in testCases)
            {
                Length firstLength = new Length(test.FirstValue, test.FirstUnit);
                Length secondLength = new Length(test.SecondValue, test.SecondUnit);

                Length result = Length.Add(firstLength, secondLength, test.Target);

                double firstInTarget = Length.Convert(test.FirstValue, test.FirstUnit, test.Target, 6);
                double secondInTarget = Length.Convert(test.SecondValue, test.SecondUnit, test.Target, 6);
                // Round expected to 2 decimals to match Length.Add behavior.
                double expected = Math.Round(firstInTarget + secondInTarget, 2);

                Assert.AreEqual(expected, result.Value, smallEpsilon,
                    $"Precision check failed for {test.FirstUnit} + {test.SecondUnit} in {test.Target}");
            }
        }
    }
}

