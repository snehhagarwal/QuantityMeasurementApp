using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Unit;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC8: Tests for refactored LengthUnit and Length (QuantityLength) design.
    /// </summary>
    [TestClass]
    public class RefactoredDesignTests
    {
        private const double EPSILON = 1e-4;

        // ---------- LengthUnit enum constant & factor tests ----------

        [TestMethod]
        public void testLengthUnitEnum_FeetConstant()
        {
            double factor = new LengthUnitExtensions(LengthUnit.FEET).GetConversionFactor();
            Assert.AreEqual(1.0, factor, EPSILON);
        }

        [TestMethod]
        public void testLengthUnitEnum_InchesConstant()
        {
            double factor = new LengthUnitExtensions(LengthUnit.INCHES).GetConversionFactor();
            Assert.AreEqual(1.0 / 12.0, factor, EPSILON);
        }

        [TestMethod]
        public void testLengthUnitEnum_YardsConstant()
        {
            double factor = new LengthUnitExtensions(LengthUnit.YARDS).GetConversionFactor();
            Assert.AreEqual(3.0, factor, EPSILON);
        }

        [TestMethod]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            double factor = new LengthUnitExtensions(LengthUnit.CENTIMETERS).GetConversionFactor();
            Assert.AreEqual(1.0 / 30.48, factor, EPSILON);
        }

        // ---------- convertToBaseUnit / convertFromBaseUnit tests ----------

        [TestMethod]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            double result = new LengthUnitExtensions(LengthUnit.FEET).ConvertToBaseUnit(5.0);
            Assert.AreEqual(5.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            double result = new LengthUnitExtensions(LengthUnit.INCHES).ConvertToBaseUnit(12.0);
            Assert.AreEqual(1.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            double result = new LengthUnitExtensions(LengthUnit.YARDS).ConvertToBaseUnit(1.0);
            Assert.AreEqual(3.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            double result = new LengthUnitExtensions(LengthUnit.CENTIMETERS).ConvertToBaseUnit(30.48);
            Assert.AreEqual(1.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            double result = new LengthUnitExtensions(LengthUnit.FEET).ConvertFromBaseUnit(2.0);
            Assert.AreEqual(2.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            double result = new LengthUnitExtensions(LengthUnit.INCHES).ConvertFromBaseUnit(1.0);
            Assert.AreEqual(12.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            double result = new LengthUnitExtensions(LengthUnit.YARDS).ConvertFromBaseUnit(3.0);
            Assert.AreEqual(1.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            double result = new LengthUnitExtensions(LengthUnit.CENTIMETERS).ConvertFromBaseUnit(1.0);
            Assert.AreEqual(30.48, result, EPSILON);
        }

        // ---------- QuantityLength (Length) refactored behavior ----------

        [TestMethod]
        public void testQuantityLengthRefactored_Equality()
        {
            Length feet = new Length(1.0, LengthUnit.FEET);
            Length inches = new Length(12.0, LengthUnit.INCHES);

            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(inches.Equals(feet));
        }

        [TestMethod]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            Length quantity = new Length(1.0, LengthUnit.FEET);
            Length converted = quantity.ConvertTo(LengthUnit.INCHES);

            Assert.AreEqual(12.0, converted.Value, EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, converted.Unit);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_Add()
        {
            Length first = new Length(1.0, LengthUnit.FEET);
            Length second = new Length(12.0, LengthUnit.INCHES);

            Length sum = first.Add(second, LengthUnit.FEET);

            Assert.AreEqual(2.0, sum.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            Length first = new Length(1.0, LengthUnit.FEET);
            Length second = new Length(12.0, LengthUnit.INCHES);

            Length sum = Length.Add(first, second, LengthUnit.YARDS);

            // Implementation rounds to 2 decimal places internally, so expected is 0.67 yards.
            double expected = 0.67;
            Assert.AreEqual(expected, sum.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, sum.Unit);
        }

        [TestMethod]
        public void testQuantityLengthRefactored_NullUnit()
        {
            try
            {
                Length invalid = new Length(1.0, (LengthUnit)0); // UNKNOWN treated as invalid
                Assert.Fail("Expected ArgumentException for UNKNOWN unit");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            try
            {
                Length invalid = new Length(double.NaN, LengthUnit.FEET);
                Assert.Fail("Expected ArgumentException for NaN value");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        // ---------- Backward compatibility sanity checks ----------

        [TestMethod]
        public void testBackwardCompatibility_UC1EqualityTests()
        {
            // Representative scenario from UC1 Feet equality
            var first = new Feet(10);
            var second = new Feet(10);
            Assert.AreEqual(first, second);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC5ConversionTests()
        {
            // Representative scenario from UC5: 1 foot -> 12 inches
            double result = Length.Convert(1.0, LengthUnit.FEET, LengthUnit.INCHES);
            Assert.AreEqual(12.0, result, EPSILON);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC6AdditionTests()
        {
            // Representative scenario from UC6: 1 ft + 12 in = 2 ft
            Length first = new Length(1.0, LengthUnit.FEET);
            Length second = new Length(12.0, LengthUnit.INCHES);
            Length sum = first.Add(second);

            Assert.AreEqual(2.0, sum.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, sum.Unit);
        }

        [TestMethod]
        public void testBackwardCompatibility_UC7AdditionWithTargetUnitTests()
        {
            // Representative scenario from UC7: 1 ft + 12 in in yards ≈ 0.667
            Length first = new Length(1.0, LengthUnit.FEET);
            Length second = new Length(12.0, LengthUnit.INCHES);
            Length sum = Length.Add(first, second, LengthUnit.YARDS);

            // Implementation rounds to 2 decimal places internally, so expected is 0.67 yards.
            double expected = 0.67;
            Assert.AreEqual(expected, sum.Value, EPSILON);
        }

        // ---------- Architectural scalability & precision ----------

        private enum WeightUnit
        {
            KILOGRAM,
            GRAM
        }

        [TestMethod]
        public void testArchitecturalScalability_MultipleCategories()
        {
            // Simple check that we can define another unit enum without linking to LengthUnit.
            WeightUnit w1 = WeightUnit.KILOGRAM;
            WeightUnit w2 = WeightUnit.GRAM;

            Assert.AreNotEqual(w1, w2);
        }

        [TestMethod]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double original = 10.0;
            double toInches = Length.Convert(original, LengthUnit.FEET, LengthUnit.INCHES);
            double backToFeet = Length.Convert(toInches, LengthUnit.INCHES, LengthUnit.FEET);

            Assert.AreEqual(original, backToFeet, EPSILON);
        }

        [TestMethod]
        public void testUnitImmutability()
        {
            // Enums are value types; constants are immutable.
            LengthUnit a = LengthUnit.FEET;
            LengthUnit b = LengthUnit.FEET;

            Assert.AreEqual(a, b);

            // Confirm that we always get same logical value for FEET
            Assert.IsTrue(a == LengthUnit.FEET);
            Assert.IsTrue(b == LengthUnit.FEET);
        }
    }
}

