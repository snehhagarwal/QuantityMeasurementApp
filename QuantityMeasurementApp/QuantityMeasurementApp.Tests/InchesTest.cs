using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// MSTest class for Inches entity and service.
    /// Tests equality, validation and tolerance.
    /// </summary>
    [TestClass]
    public class InchesTests
    {
        private Inches firstMeasurement;
        private Inches secondMeasurement;
        private Inches thirdMeasurement;
        private InchesService measurementService;
        /// <summary>
        /// Runs before every test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            measurementService = new InchesService();
        }
        // UC1 Equality Contract Tests
        [TestMethod]
        public void testEquality_SameValue()
        {
            firstMeasurement = new Inches(10);
            secondMeasurement = new Inches(10);

            Assert.AreEqual(firstMeasurement, secondMeasurement);
        }
        [TestMethod]
        public void testEquality_DifferentValue()
        {
            firstMeasurement = new Inches(10);
            secondMeasurement = new Inches(20);

            Assert.AreNotEqual(firstMeasurement, secondMeasurement);
        }
        [TestMethod]
        public void testEquality_NullComparison()
        {
            firstMeasurement = new Inches(5);
            Assert.IsFalse(firstMeasurement.Equals(null));
        }
        [TestMethod]
        public void testEquality_SameReference()
        {
            firstMeasurement = new Inches(15);
            Assert.AreEqual(firstMeasurement, firstMeasurement);
        }
        [TestMethod]
        public void testEquality_TypeSafety()
        {
            firstMeasurement = new Inches(12);
            Assert.IsFalse(firstMeasurement.Equals("abc"));
        }
        // Validation Tests
        [TestMethod]
        public void testValidation_NegativeValue()
        {
            Assert.Throws<ArgumentException>(() => new Inches(-10));
        }

        [TestMethod]
        public void testValidation_NaN()
        {
            Assert.Throws<ArgumentException>(() => new Inches(double.NaN));
        }

        [TestMethod]
        public void testValidation_Infinity()
        {
            Assert.Throws<ArgumentException>(() => new Inches(double.PositiveInfinity));
        }
        // Tolerance Tests
        [TestMethod]
        public void testTolerance_WithinRange()
        {
            firstMeasurement = new Inches(25);
            secondMeasurement = new Inches(25.4);
            bool result =measurementService.AreEqualWithTolerance(firstMeasurement,secondMeasurement,0.5);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void testTolerance_OutsideRange()
        {
            firstMeasurement = new Inches(25);
            secondMeasurement = new Inches(30);
            bool result =measurementService.AreEqualWithTolerance(firstMeasurement,secondMeasurement,1);
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void testTolerance_NegativeTolerance()
        {
            firstMeasurement = new Inches(10);
            secondMeasurement = new Inches(10);
            Assert.Throws<ArgumentException>(() =>measurementService.AreEqualWithTolerance(firstMeasurement,secondMeasurement,-1));
        }
    }
}