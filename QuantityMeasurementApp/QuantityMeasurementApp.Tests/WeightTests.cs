using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Unit;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC9: Tests for Weight measurement equality, conversion, and addition.
    /// </summary>
    [TestClass]
    public class WeightTests
    {
        private const double EPSILON = 1e-4;

        // Equality tests

        [TestMethod]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(1.0, WeightUnit.KILOGRAM);
            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(2.0, WeightUnit.KILOGRAM);
            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            Weight kg = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight g = new Weight(1000.0, WeightUnit.GRAM);
            Assert.IsTrue(kg.Equals(g));
        }

        [TestMethod]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            Weight g = new Weight(1000.0, WeightUnit.GRAM);
            Weight kg = new Weight(1.0, WeightUnit.KILOGRAM);
            Assert.IsTrue(g.Equals(kg));
        }

        [TestMethod]
        public void testEquality_WeightVsLength_Incompatible()
        {
            Weight weight = new Weight(1.0, WeightUnit.KILOGRAM);
            Length length = new Length(1.0, LengthUnit.FEET);
            Assert.IsFalse(weight.Equals(length));
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            Weight weight = new Weight(1.0, WeightUnit.KILOGRAM);
            Assert.IsFalse(weight.Equals(null));
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            Weight weight = new Weight(1.0, WeightUnit.KILOGRAM);
            Assert.IsTrue(weight.Equals(weight));
        }

        [TestMethod]
        public void testEquality_NullUnit()
        {
            try
            {
                Weight invalid = new Weight(1.0, (WeightUnit)0); // UNKNOWN treated as invalid
                Assert.Fail("Expected ArgumentException for UNKNOWN unit");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void testEquality_TransitiveProperty()
        {
            Weight a = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(1000.0, WeightUnit.GRAM);
            Weight c = new Weight(1.0, WeightUnit.KILOGRAM);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        [TestMethod]
        public void testEquality_ZeroValue()
        {
            Weight zeroKg = new Weight(0.0, WeightUnit.KILOGRAM);
            Weight zeroG = new Weight(0.0, WeightUnit.GRAM);
            Assert.IsTrue(zeroKg.Equals(zeroG));
        }

        [TestMethod]
        public void testEquality_NegativeWeight()
        {
            Weight a = new Weight(-1.0, WeightUnit.KILOGRAM);
            Weight b = new Weight(-1000.0, WeightUnit.GRAM);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_LargeWeightValue()
        {
            Weight a = new Weight(1_000_000.0, WeightUnit.GRAM);
            Weight b = new Weight(1000.0, WeightUnit.KILOGRAM);
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_SmallWeightValue()
        {
            Weight a = new Weight(0.001, WeightUnit.KILOGRAM);
            Weight b = new Weight(1.0, WeightUnit.GRAM);
            Assert.IsTrue(a.Equals(b));
        }

        // Conversion tests

        [TestMethod]
        public void testConversion_PoundToKilogram()
        {
            Weight w = new Weight(2.20462, WeightUnit.POUND);
            Weight converted = w.ConvertTo(WeightUnit.KILOGRAM, decimalPlaces: 5);
            Assert.AreEqual(1.0, converted.Value, 1e-3);
            Assert.AreEqual(WeightUnit.KILOGRAM, converted.Unit);
        }

        [TestMethod]
        public void testConversion_KilogramToPound()
        {
            Weight w = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight converted = w.ConvertTo(WeightUnit.POUND, decimalPlaces: 5);

            double expected = 1.0 / 0.453592; // ≈ 2.20462 lb
            Assert.AreEqual(expected, converted.Value, 1e-3);
            Assert.AreEqual(WeightUnit.POUND, converted.Unit);
        }

        [TestMethod]
        public void testConversion_SameUnit()
        {
            Weight w = new Weight(5.0, WeightUnit.KILOGRAM);
            Weight converted = w.ConvertTo(WeightUnit.KILOGRAM);
            Assert.AreEqual(5.0, converted.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, converted.Unit);
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            Weight w = new Weight(0.0, WeightUnit.KILOGRAM);
            Weight converted = w.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(0.0, converted.Value, EPSILON);
            Assert.AreEqual(WeightUnit.GRAM, converted.Unit);
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            Weight w = new Weight(-1.0, WeightUnit.KILOGRAM);
            Weight converted = w.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(-1000.0, converted.Value, EPSILON);
            Assert.AreEqual(WeightUnit.GRAM, converted.Unit);
        }

        [TestMethod]
        public void testConversion_RoundTrip()
        {
            Weight w = new Weight(1.5, WeightUnit.KILOGRAM);
            Weight inGrams = w.ConvertTo(WeightUnit.GRAM, decimalPlaces: 6);
            Weight backToKg = inGrams.ConvertTo(WeightUnit.KILOGRAM, decimalPlaces: 6);

            Assert.AreEqual(1.5, backToKg.Value, 1e-3);
            Assert.AreEqual(WeightUnit.KILOGRAM, backToKg.Unit);
        }

        // Addition tests

        [TestMethod]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(2.0, WeightUnit.KILOGRAM);
            Weight sum = first.Add(second);

            Assert.AreEqual(3.0, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
        }

        [TestMethod]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(1000.0, WeightUnit.GRAM);
            Weight sum = first.Add(second);

            Assert.AreEqual(2.0, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
        }

        [TestMethod]
        public void testLengthUnitEnum_InchesConstant()
        {
            // As per description: cross-unit addition result in first operand's unit.
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(1000.0, WeightUnit.GRAM);
            Weight sum = first.Add(second);

            Assert.AreEqual(2.0, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
        }

        [TestMethod]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            Weight first = new Weight(2.20462, WeightUnit.POUND);
            Weight second = new Weight(1.0, WeightUnit.KILOGRAM);

            Weight sum = first.Add(second);

            double expectedBaseKg = 2.20462 * 0.453592 + 1.0;
            double expectedPounds = expectedBaseKg / 0.453592;
            double roundedExpected = Math.Round(expectedPounds, 2);

            Assert.AreEqual(roundedExpected, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.POUND, sum.Unit);
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Kilogram()
        {
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(1000.0, WeightUnit.GRAM);
            Weight sum = first.Add(second, WeightUnit.GRAM);

            Assert.AreEqual(2000.0, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.GRAM, sum.Unit);
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            Weight first = new Weight(1.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(1000.0, WeightUnit.GRAM);

            Weight sumFirst = first.Add(second); // in KILOGRAM
            Weight sumSecond = second.Add(first); // in GRAM

            // They should represent the same physical quantity
            Assert.IsTrue(sumFirst.Equals(sumSecond));
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            Weight first = new Weight(5.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(0.0, WeightUnit.GRAM);

            Weight sum = first.Add(second);

            Assert.AreEqual(5.0, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            Weight first = new Weight(5.0, WeightUnit.KILOGRAM);
            Weight second = new Weight(-2000.0, WeightUnit.GRAM);

            Weight sum = first.Add(second);

            Assert.AreEqual(3.0, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            double large = 1e6;
            Weight first = new Weight(large, WeightUnit.KILOGRAM);
            Weight second = new Weight(large, WeightUnit.KILOGRAM);

            Weight sum = first.Add(second);

            Assert.AreEqual(large * 2, sum.Value, EPSILON);
            Assert.AreEqual(WeightUnit.KILOGRAM, sum.Unit);
        }
    }
}

