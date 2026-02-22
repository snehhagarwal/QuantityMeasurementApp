using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityTests
    {

        private Length first;
        private Length second;
        private Length third;

        /// <summary>
        /// 1. Reflexive Property
        /// Quantity equals itself
        /// </summary>
        [TestMethod]
        public void testEquality_SameReference()
        {
            first = new Length(1.0, LengthUnit.FEET);

            Assert.IsTrue(first.Equals(first));
        }

        /// <summary>
        /// 2. Feet to Feet Same Value
        /// </summary>
        [TestMethod]
        public void testEquality_FeetToFeet_SameValue()
        {
            first = new Length(1.0, LengthUnit.FEET);
            second = new Length(1.0, LengthUnit.FEET);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// 3. Inch to Inch Same Value
        /// </summary>
        [TestMethod]
        public void testEquality_InchToInch_SameValue()
        {
            first = new Length(1.0, LengthUnit.INCHES);
            second = new Length(1.0, LengthUnit.INCHES);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// 4. Feet to Inch Equivalent Value
        /// </summary>
        [TestMethod]
        public void testEquality_FeetToInch_EquivalentValue()
        {
            first = new Length(1.0, LengthUnit.FEET);
            second = new Length(12.0, LengthUnit.INCHES);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// 5. Inch to Feet Equivalent Value
        /// </summary>
        [TestMethod]
        public void testEquality_InchToFeet_EquivalentValue()
        {
            first = new Length(12.0, LengthUnit.INCHES);
            second = new Length(1.0, LengthUnit.FEET);

            Assert.AreEqual(first, second);
        }

        /// <summary>
        /// 6. Feet to Feet Different Value
        /// </summary>
        [TestMethod]
        public void testEquality_FeetToFeet_DifferentValue()
        {
            first = new Length(1.0, LengthUnit.FEET);
            second = new Length(2.0, LengthUnit.FEET);

            Assert.AreNotEqual(first, second);
        }

        /// <summary>
        /// 7. Inch to Inch Different Value
        /// </summary>
        [TestMethod]
        public void testEquality_InchToInch_DifferentValue()
        {
            first = new Length(1.0, LengthUnit.INCHES);
            second = new Length(2.0, LengthUnit.INCHES);

            Assert.AreNotEqual(first, second);
        }

        /// <summary>
        /// 8. Null Comparison
        /// </summary>
        [TestMethod]
        public void testEquality_NullComparison()
        {
            first = new Length(1.0, LengthUnit.FEET);

            Assert.IsFalse(first.Equals(null));
        }

        /// <summary>
        /// 9. Invalid Unit Test
        /// </summary>
        [TestMethod]
        public void testEquality_InvalidUnit()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Length invalid = new Length(10, (LengthUnit)999);
            });
        }

        /// <summary>
        /// 10. Null Unit Test
        /// </summary>
        [TestMethod]
        public void testEquality_NullUnit()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Length invalid = new Length(10, default);
            });
        }

    }
}