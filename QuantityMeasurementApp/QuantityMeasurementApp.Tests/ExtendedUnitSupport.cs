using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Service;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// Unit Test Class for Generic Length Entity (UC4).
    ///
    /// Purpose:
    /// Validate equality comparison across Feet, Inches, Yards, Centimeters.
    ///
    /// Covers:
    /// • Cross-unit equality
    /// • Unit conversions
    /// • Symmetry
    /// • Transitive property
    /// • Null handling
    /// • Reflexive property
    ///
    /// Framework Used: MSTest
    /// </summary>

    [TestClass]
    public class ExtendedUnitSupport
    {
        private Length firstMeasurement;
        private Length secondMeasurement;
        private Length thirdMeasurement;
        private LengthService measurementService;

        /// <summary>
        /// Initializes service before each test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            measurementService = new LengthService();
        }

        /// <summary>
        /// testEquality_YardToYard_SameValue
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenSameYardMeasurements_ShouldReturnTrue()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            secondMeasurement = new Length(1.0, LengthUnit.YARDS);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_YardToYard_DifferentValue
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenDifferentYardMeasurements_ShouldReturnFalse()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            secondMeasurement = new Length(2.0, LengthUnit.YARDS);
            Assert.IsFalse(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_YardToFeet_EquivalentValue
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenYardAndFeetEquivalent_ShouldReturnTrue()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            secondMeasurement = new Length(3.0, LengthUnit.FEET);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_FeetToYard_EquivalentValue
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenFeetAndYardEquivalent_ShouldReturnTrue()
        {
            firstMeasurement = new Length(3.0, LengthUnit.FEET);
            secondMeasurement = new Length(1.0, LengthUnit.YARDS);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_YardToInches_EquivalentValue
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenYardAndInchesEquivalent_ShouldReturnTrue()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            secondMeasurement = new Length(36.0, LengthUnit.INCHES);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_InchesToYard_EquivalentValue
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenInchesAndYardEquivalent_ShouldReturnTrue()
        {
            firstMeasurement = new Length(36.0, LengthUnit.INCHES);
            secondMeasurement = new Length(1.0, LengthUnit.YARDS);

            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_YardToFeet_NonEquivalentValue
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenYardAndFeetNonEquivalent_ShouldReturnFalse()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            secondMeasurement = new Length(2.0, LengthUnit.FEET);

            Assert.IsFalse(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_centimetersToInches_EquivalentValue
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenCentimeterAndInchesEquivalent_ShouldReturnTrue()
        {
            firstMeasurement = new Length(1.0, LengthUnit.CENTIMETERS);
            secondMeasurement = new Length(0.393701, LengthUnit.INCHES);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_centimetersToFeet_NonEquivalentValue
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenCentimeterAndFeetNonEquivalent_ShouldReturnFalse()
        {
            firstMeasurement = new Length(1.0, LengthUnit.CENTIMETERS);
            secondMeasurement = new Length(1.0, LengthUnit.FEET);

            Assert.IsFalse(firstMeasurement.Equals(secondMeasurement));
        }

        /// <summary>
        /// testEquality_MultiUnit_TransitiveProperty
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenMultiUnitTransitiveProperty_ShouldReturnTrue()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            secondMeasurement = new Length(3.0, LengthUnit.FEET);
            thirdMeasurement = new Length(36.0, LengthUnit.INCHES);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement)&&secondMeasurement.Equals(thirdMeasurement)&&firstMeasurement.Equals(thirdMeasurement));
        }

        /// <summary>
        /// testEquality_YardWithNullUnit
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenYardComparedWithNull_ShouldReturnFalse()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            Assert.IsFalse(firstMeasurement.Equals(null));
        }

        /// <summary>
        /// testEquality_YardSameReference
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenSameYardReference_ShouldReturnTrue()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            Assert.AreEqual(firstMeasurement, firstMeasurement);
        }

        /// <summary>
        /// testEquality_YardNullComparison
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenYardAndNullComparison_ShouldReturnFalse()
        {
            firstMeasurement = new Length(1.0, LengthUnit.YARDS);
            Assert.IsFalse(firstMeasurement.Equals(null));
        }

        /// <summary>
        /// testEquality_CentimetersWithNullUnit
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenCentimeterComparedWithNull_ShouldReturnFalse()
        {
            firstMeasurement = new Length(1.0, LengthUnit.CENTIMETERS);
            Assert.IsFalse(firstMeasurement.Equals(null));
        }

        /// <summary>
        /// testEquality_CentimetersSameReference
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenSameCentimeterReference_ShouldReturnTrue()
        {
            firstMeasurement = new Length(2.0, LengthUnit.CENTIMETERS);
            Assert.AreEqual(firstMeasurement, firstMeasurement);
        }

        /// <summary>
        /// testEquality_CentimetersNullComparison
        /// Expected: False
        /// </summary>
        [TestMethod]
        public void GivenCentimeterAndNullComparison_ShouldReturnFalse()
        {
            firstMeasurement = new Length(2.0, LengthUnit.CENTIMETERS);
            Assert.IsFalse(firstMeasurement.Equals(null));
        }

        /// <summary>
        /// testEquality_AllUnits_ComplexScenario
        /// Expected: True
        /// </summary>
        [TestMethod]
        public void GivenComplexMultiUnitScenario_ShouldReturnTrue()
        {
            firstMeasurement = new Length(2.0, LengthUnit.YARDS);
            secondMeasurement = new Length(6.0, LengthUnit.FEET);
            thirdMeasurement = new Length(72.0, LengthUnit.INCHES);
            Assert.IsTrue(firstMeasurement.Equals(secondMeasurement)&&secondMeasurement.Equals(thirdMeasurement)&&firstMeasurement.Equals(thirdMeasurement));
        }
    }
}