using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementModel.Units;
using QuantityMeasurementModel.Enums;
using QuantityMeasurementBusinessLayer.Unit;
using QuantityMeasurementBusinessLayer.Service;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// Unit Test Class for Feet Entity and FeetService.
    /// 
    /// Purpose:
    /// Validate equality comparison and tolerance logic of Feet measurement.
    /// 
    /// Covers:
    /// • Equality scenarios
    /// • Invalid input validation
    /// • Exception handling
    /// • Edge cases (NaN, Infinity, negative, large values)
    /// • Tolerance comparison feature
    /// 
    /// Framework Used: MSTest
    /// </summary>
    [TestClass]
    public class FeetTests
    {
        // Global objects used across test methods
        private Feet firstMeasurement;
        private Feet secondMeasurement;
        private Feet thirdMeasurement;
        private FeetService measurementService;
        /// <summary>
        /// Initializes service before each test execution.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            measurementService = new FeetService();
        }
        /// <summary>
        /// Test equality when both measurements are same.
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenSameFeetMeasurements_ShouldReturnTrue()
        {
            firstMeasurement = new Feet(10);
            secondMeasurement = new Feet(10);
            Assert.AreEqual(firstMeasurement, secondMeasurement);
        }

        /// <summary>
        /// Test equality when measurements are different.
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenDifferentFeetMeasurements_ShouldReturnFalse()
        {
            firstMeasurement = new Feet(10);
            secondMeasurement = new Feet(20);

            Assert.AreNotEqual(firstMeasurement, secondMeasurement);
        }
        /// <summary>
        /// Test comparison with null.
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenNullMeasurement_ShouldReturnFalse()
        {
            firstMeasurement = new Feet(5);
            Assert.IsFalse(firstMeasurement.Equals(null));
        }
        /// <summary>
        /// Test equality when same object reference is used.
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenSameReference_ShouldReturnTrue()
        {
            firstMeasurement = new Feet(15);
            Assert.AreEqual(firstMeasurement, firstMeasurement);
        }
        /// <summary>
        /// Test comparison with different object type.
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenDifferentObjectType_ShouldReturnFalse()
        {
            firstMeasurement = new Feet(12);
            Assert.IsFalse(firstMeasurement.Equals("invalid"));
        }
        /// <summary>
        /// Test transitive equality property.
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenTransitiveEqualMeasurements_ShouldReturnTrue()
        {
            firstMeasurement = new Feet(8);
            secondMeasurement = new Feet(8);
            thirdMeasurement = new Feet(8);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement) && secondMeasurement.Equals(thirdMeasurement) && firstMeasurement.Equals(thirdMeasurement));
        }
        /// <summary>
        /// Test equality for zero value.
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenZeroFeetMeasurement_ShouldReturnTrue()
        {
            firstMeasurement = new Feet(0);
            secondMeasurement = new Feet(0);
            Assert.AreEqual(firstMeasurement, secondMeasurement);
        }
        /// <summary>
        /// Test equality for decimal values.
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenDecimalFeetMeasurement_ShouldReturnTrue()
        {
            firstMeasurement = new Feet(12.75);
            secondMeasurement = new Feet(12.75);
            Assert.AreEqual(firstMeasurement, secondMeasurement);
        }
        /// <summary>
        /// Test constructor with negative value.
        /// Expected: Exception
        /// </summary>
        [TestMethod]
        public void GivenNegativeFeetMeasurement_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(-10));
        }
        /// <summary>
        /// Test constructor with extremely large value.
        /// Expected: Exception
        /// </summary>
        [TestMethod]
        public void GivenExtremelyLargeFeetMeasurement_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>new Feet(999999999));
        }
        /// <summary>
        /// Test constructor with maximum double value.
        /// Expected: Exception
        /// </summary>
        [TestMethod]
        public void GivenMaxDoubleFeetMeasurement_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>new Feet(double.MaxValue));
        }
        // <summary>
        // Test constructor with minimum double value.
        // Expected: Exception
        // </summary>
        [TestMethod]
        public void GivenMinDoubleFeetMeasurement_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Feet(double.MinValue));
        }
        /// <summary>
        /// Test constructor with NaN.
        /// Expected: Exception
        /// </summary>
        [TestMethod]
        public void GivenNaNFeetMeasurement_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>new Feet(double.NaN));
        }
        /// <summary>
        /// Test constructor with Infinity.
        /// Expected: Exception
        /// </summary>
        [TestMethod]
        public void GivenInfinityFeetMeasurement_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>new Feet(double.PositiveInfinity));
        }
        /// <summary>
        /// Test tolerance comparison within allowed range.
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenMeasurementsWithinTolerance_ShouldReturnTrue()
        {
            firstMeasurement = new Feet(25);
            secondMeasurement = new Feet(25.4);
            bool result=measurementService.AreEqualWithTolerance(firstMeasurement,secondMeasurement,0.5);
            Assert.IsTrue(result);
        }
        /// <summary>
        /// Test tolerance comparison outside allowed range.
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenMeasurementsOutsideTolerance_ShouldReturnFalse()
        {
            firstMeasurement = new Feet(30);
            secondMeasurement = new Feet(35);
            bool result=measurementService.AreEqualWithTolerance(firstMeasurement,secondMeasurement,1);
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Test negative tolerance value.
        /// Expected: Exception
        /// </summary>
        [TestMethod]
        public void GivenNegativeTolerance_ShouldThrowException()
        {
            firstMeasurement = new Feet(40);
            secondMeasurement = new Feet(40);

            Assert.Throws<ArgumentException>(() => measurementService.AreEqualWithTolerance(firstMeasurement,secondMeasurement,-1));
        }
    }
}