using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC6: Tests for addition of two length values.
    /// Only the requested test cases are included.
    /// </summary>
    [TestClass]
    public class LengthAdditionTests
    {
        private const double EPSILON = 0.01;

        // testAddition_SameUnit_FeetPlusFeet
        // Add (Quantity(1.0, FEET), Quantity(2.0, FEET)) should return Quantity(3.0, FEET).
        [TestMethod]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            Length first = new Length(1.0, LengthUnit.FEET);
            Length second = new Length(2.0, LengthUnit.FEET);

            Length result = first.Add(second);

            Assert.AreEqual(3.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_SameUnit_InchPlusInch
        // Add (Quantity(6.0, INCHES), Quantity(6.0, INCHES)) should return Quantity(12.0, INCHES).
        [TestMethod]
        public void testAddition_SameUnit_InchPlusInch()
        {
            Length first = new Length(6.0, LengthUnit.INCHES);
            Length second = new Length(6.0, LengthUnit.INCHES);

            Length result = first.Add(second);

            Assert.AreEqual(12.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, result.Unit);
        }

        // testAddition_CrossUnit_FeetPlusInches
        // Add (Quantity(1.0, FEET), Quantity(12.0, INCHES)) should return Quantity(2.0, FEET).
        [TestMethod]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            Length first = new Length(1.0, LengthUnit.FEET);
            Length second = new Length(12.0, LengthUnit.INCHES);

            Length result = first.Add(second); // result is in FEET (unit of first)

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_CrossUnit_InchPlusFeet
        // Add (Quantity(12.0, INCHES), Quantity(1.0, FEET)) should return Quantity(24.0, INCHES).
        [TestMethod]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            Length first = new Length(12.0, LengthUnit.INCHES);
            Length second = new Length(1.0, LengthUnit.FEET);

            Length result = first.Add(second); // result is in INCHES (unit of first)

            Assert.AreEqual(24.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.INCHES, result.Unit);
        }

        // testAddition_CrossUnit_YardPlusFeet
        // Add (Quantity(1.0, YARDS), Quantity(3.0, FEET)) should return Quantity(2.0, YARDS).
        [TestMethod]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            Length first = new Length(1.0, LengthUnit.YARDS);
            Length second = new Length(3.0, LengthUnit.FEET);

            Length result = first.Add(second); // result in YARDS

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.Unit);
        }

        // testAddition_CrossUnit_CentimeterPlusInch
        // Add (Quantity(2.54, CENTIMETERS), Quantity(1.0, INCHES)) should return Quantity(~5.08, CENTIMETERS).
        [TestMethod]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            Length first = new Length(2.54, LengthUnit.CENTIMETERS);
            Length second = new Length(1.0, LengthUnit.INCHES);

            Length result = first.Add(second); // result in CENTIMETERS

            double expected = 5.08; // 2.54 cm + 2.54 cm
            Assert.AreEqual(expected, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.CENTIMETERS, result.Unit);
        }

        // testAddition_Commutativity
        // Add (Quantity(1.0, FEET), Quantity(12.0, INCHES)) should equal add(Quantity(12.0, INCHES), Quantity(1.0, FEET)).
        [TestMethod]
        public void testAddition_Commutativity()
        {
            Length firstA = new Length(1.0, LengthUnit.FEET);
            Length secondA = new Length(12.0, LengthUnit.INCHES);
            Length resultFeetPlusInches = firstA.Add(secondA); // result in FEET

            Length firstB = new Length(12.0, LengthUnit.INCHES);
            Length secondB = new Length(1.0, LengthUnit.FEET);
            Length resultInchesPlusFeet = firstB.Add(secondB); // result in INCHES

            // They represent the same physical length so they should be equal as Length (uses base-unit equality)
            Assert.IsTrue(resultFeetPlusInches.Equals(resultInchesPlusFeet));
        }

        // testAddition_WithZero
        // Add (Quantity(5.0, FEET), Quantity(0.0, INCHES)) should return Quantity(5.0, FEET).
        [TestMethod]
        public void testAddition_WithZero()
        {
            Length first = new Length(5.0, LengthUnit.FEET);
            Length second = new Length(0.0, LengthUnit.INCHES);

            Length result = first.Add(second);

            Assert.AreEqual(5.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_NegativeValues
        // Add (Quantity(5.0, FEET), Quantity(-2.0, FEET)) should return Quantity(3.0, FEET).
        [TestMethod]
        public void testAddition_NegativeValues()
        {
            Length first = new Length(5.0, LengthUnit.FEET);
            Length second = new Length(-2.0, LengthUnit.FEET);

            Length result = first.Add(second);

            Assert.AreEqual(3.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_NullSecondOperand
        // Add (Quantity(1.0, FEET), null) should throw IllegalArgumentException or NullPointerException.
        [TestMethod]
        public void testAddition_NullSecondOperand()
        {
            Length first = new Length(1.0, LengthUnit.FEET);
            Length? second = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                first.Add(second!);
            });
        }

        // testAddition_LargeValues
        // Add (Quantity(1e6, FEET), Quantity(1e6, FEET)) should return Quantity(2e6, FEET).
        [TestMethod]
        public void testAddition_LargeValues()
        {
            double large = 1_000_000.0;

            Length first = new Length(large, LengthUnit.FEET);
            Length second = new Length(large, LengthUnit.FEET);

            Length result = first.Add(second);

            Assert.AreEqual(large * 2, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_SmallValues
        // Add (Quantity(0.001, FEET), Quantity(0.002, FEET)) should return Quantity(~0.003, FEET) (within epsilon).
        [TestMethod]
        public void testAddition_SmallValues()
        {
            Length first = new Length(0.001, LengthUnit.FEET);
            Length second = new Length(0.002, LengthUnit.FEET);

            // Use more decimal places so we do not lose precision.
            Length result = first.Add(second, decimalPlaces: 6);

            double expected = 0.003;
            Assert.AreEqual(expected, result.Value, 1e-6);
            Assert.AreEqual(LengthUnit.FEET, result.Unit);
        }

        // testAddition_CrossUnit_YardPlusInches
        // Add (Quantity(1.0, YARDS), Quantity(36.0, INCHES)) should return Quantity(2.0, YARDS).
        [TestMethod]
        public void testAddition_CrossUnit_YardPlusInches()
        {
            Length first = new Length(1.0, LengthUnit.YARDS);
            Length second = new Length(36.0, LengthUnit.INCHES);

            Length result = first.Add(second); // result in YARDS (unit of first)

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.YARDS, result.Unit);
        }
    }
}

