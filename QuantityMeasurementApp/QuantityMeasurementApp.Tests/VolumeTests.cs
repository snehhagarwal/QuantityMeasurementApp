using QuantityMeasurementBusinessLayer.Service;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Units;
using QuantityMeasurementModel.Enums;
using QuantityMeasurementBusinessLayer.Unit;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC11: Tests for Volume measurement equality, conversion, and addition.
    /// Covers Quantity&lt;VolumeUnitMeasurable&gt; with LITRE, MILLILITRE, and GALLON units.
    /// Confirms that the UC10 generic architecture scales seamlessly to a third
    /// measurement category with zero changes to Quantity&lt;TUnit&gt; or IMeasurable.
    /// </summary>
    [TestClass]
    public class VolumeTests
    {
        private const double EPSILON = 1e-4;
        private const double LitresPerGallon = 3.78541; // 1 US gallon ≈ 3.78541 L

        // Equality tests

        [TestMethod]
        public void testEquality_LitreToLitre_SameValue()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(2.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.IsTrue(litre.Equals(millilitre));
        }

        [TestMethod]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsTrue(millilitre.Equals(litre));
        }

        [TestMethod]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            // 1 litre ≈ 0.264172 gallon  (1 / 3.78541)
            double gallonEquiv = 1.0 / LitresPerGallon;
            var litre  = new Quantity<VolumeUnitMeasurable>(1.0,         new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var gallon = new Quantity<VolumeUnitMeasurable>(gallonEquiv, new VolumeUnitMeasurable(VolumeUnit.GALLON));
            Assert.IsTrue(litre.Equals(gallon));
        }

        [TestMethod]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            // 1 gallon ≈ 3.78541 litres
            var gallon = new Quantity<VolumeUnitMeasurable>(1.0,           new VolumeUnitMeasurable(VolumeUnit.GALLON));
            var litre  = new Quantity<VolumeUnitMeasurable>(LitresPerGallon, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsTrue(gallon.Equals(litre));
        }

        [TestMethod]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            var volume = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var length = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.IsFalse(volume.Equals(length));
        }

        [TestMethod]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            var volume = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.IsFalse(volume.Equals(weight));
        }

        [TestMethod]
        public void testEquality_NullComparison()
        {
            var litre = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsFalse(litre.Equals(null));
        }

        [TestMethod]
        public void testEquality_SameReference()
        {
            var litre = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsTrue(litre.Equals(litre));
        }

        [TestMethod]
        public void testEquality_NullUnit()
        {
            try
            {
                // VolumeUnit.UNKNOWN (value 0) is treated as invalid — triggers exception on use
                var invalid = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.UNKNOWN));
                invalid.ToBaseUnit(); // forces ConvertToBaseUnit — throws on UNKNOWN
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
            var a = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var b = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var c = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        [TestMethod]
        public void testEquality_ZeroValue()
        {
            var zeroLitre      = new Quantity<VolumeUnitMeasurable>(0.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var zeroMillilitre = new Quantity<VolumeUnitMeasurable>(0.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.IsTrue(zeroLitre.Equals(zeroMillilitre));
        }

        [TestMethod]
        public void testEquality_NegativeVolume()
        {
            var a = new Quantity<VolumeUnitMeasurable>(-1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var b = new Quantity<VolumeUnitMeasurable>(-1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_LargeVolumeValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(1_000_000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var b = new Quantity<VolumeUnitMeasurable>(1000.0,      new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void testEquality_SmallVolumeValue()
        {
            var a = new Quantity<VolumeUnitMeasurable>(0.001, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var b = new Quantity<VolumeUnitMeasurable>(1.0,   new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.IsTrue(a.Equals(b));
        }

        // Conversion tests

        [TestMethod]
        public void testConversion_LitreToMillilitre()
        {
            var litre   = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result  = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(1000.0,       result.Value,           EPSILON);
            Assert.AreEqual("MILLILITRE", result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_MillilitreToLitre()
        {
            var ml     = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var result = ml.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(1.0,    result.Value,           EPSILON);
            Assert.AreEqual("LITRE", result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_GallonToLitre()
        {
            var gallon = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.GALLON));
            var result = gallon.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.LITRE), decimalPlaces: 5);
            Assert.AreEqual(LitresPerGallon, result.Value, EPSILON);
            Assert.AreEqual("LITRE",          result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_LitreToGallon()
        {
            var litre  = new Quantity<VolumeUnitMeasurable>(LitresPerGallon, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.GALLON), decimalPlaces: 5);
            Assert.AreEqual(1.0,     result.Value, EPSILON);
            Assert.AreEqual("GALLON", result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_MillilitreToGallon()
        {
            // 1000 mL = 1 L ≈ 0.264172 gallons
            var ml     = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var result = ml.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.GALLON), decimalPlaces: 6);
            double expected = 1.0 / LitresPerGallon; // ≈ 0.264172
            Assert.AreEqual(expected, result.Value, EPSILON);
            Assert.AreEqual("GALLON",  result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_SameUnit()
        {
            var litre  = new Quantity<VolumeUnitMeasurable>(5.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(5.0,    result.Value,           EPSILON);
            Assert.AreEqual("LITRE", result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_ZeroValue()
        {
            var litre  = new Quantity<VolumeUnitMeasurable>(0.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(0.0,          result.Value,           EPSILON);
            Assert.AreEqual("MILLILITRE", result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_NegativeValue()
        {
            var litre  = new Quantity<VolumeUnitMeasurable>(-1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(-1000.0,      result.Value,           EPSILON);
            Assert.AreEqual("MILLILITRE", result.Unit.GetUnitName());
        }

        [TestMethod]
        public void testConversion_RoundTrip()
        {
            var litre      = new Quantity<VolumeUnitMeasurable>(1.5, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var inML       = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.MILLILITRE), decimalPlaces: 6);
            var backToLitre = inML.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.LITRE),      decimalPlaces: 6);
            Assert.AreEqual(1.5,    backToLitre.Value, 1e-3);
            Assert.AreEqual("LITRE", backToLitre.Unit.GetUnitName());
        }

        // Addition tests

        [TestMethod]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(2.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var sum    = first.Add(second);

            Assert.AreEqual(3.0,    sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(500.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var second = new Quantity<VolumeUnitMeasurable>(500.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum    = first.Add(second);

            Assert.AreEqual(1000.0,       sum.Value,           EPSILON);
            Assert.AreEqual("MILLILITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum        = litre.Add(millilitre);

            Assert.AreEqual(2.0,    sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var sum        = millilitre.Add(litre);

            Assert.AreEqual(2000.0,       sum.Value,           EPSILON);
            Assert.AreEqual("MILLILITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            // 1 GALLON + 3.78541 LITRE  = 2 GALLON (since 3.78541 L = 1 gallon)
            var gallon = new Quantity<VolumeUnitMeasurable>(1.0,           new VolumeUnitMeasurable(VolumeUnit.GALLON));
            var litre  = new Quantity<VolumeUnitMeasurable>(LitresPerGallon, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var sum    = gallon.Add(litre);

            Assert.AreEqual(2.0,     sum.Value, EPSILON);
            Assert.AreEqual("GALLON", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum        = litre.Add(millilitre, new VolumeUnitMeasurable(VolumeUnit.LITRE));

            Assert.AreEqual(2.0,    sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum        = litre.Add(millilitre, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));

            Assert.AreEqual(2000.0,       sum.Value,           EPSILON);
            Assert.AreEqual("MILLILITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            // 3.78541 L + 3.78541 L = 7.57082 L = 2 gallons
            var first  = new Quantity<VolumeUnitMeasurable>(LitresPerGallon, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(LitresPerGallon, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var sum    = first.Add(second, new VolumeUnitMeasurable(VolumeUnit.GALLON));

            Assert.AreEqual(2.0,     sum.Value, EPSILON);
            Assert.AreEqual("GALLON", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_Commutativity()
        {
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));

            var sumForward  = litre.Add(millilitre);      // result in LITRE
            var sumReversed = millilitre.Add(litre);      // result in MILLILITRE

            // Both sums represent the same physical quantity
            Assert.IsTrue(sumForward.Equals(sumReversed));
        }

        [TestMethod]
        public void testAddition_WithZero()
        {
            var litre = new Quantity<VolumeUnitMeasurable>(5.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var zero  = new Quantity<VolumeUnitMeasurable>(0.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum   = litre.Add(zero);

            Assert.AreEqual(5.0,    sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var litre = new Quantity<VolumeUnitMeasurable>(5.0,     new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var neg   = new Quantity<VolumeUnitMeasurable>(-2000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum   = litre.Add(neg);

            Assert.AreEqual(3.0,    sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_LargeValues()
        {
            double large = 1e6;
            var first  = new Quantity<VolumeUnitMeasurable>(large, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(large, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var sum    = first.Add(second);

            Assert.AreEqual(large * 2, sum.Value,           EPSILON);
            Assert.AreEqual("LITRE",    sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testAddition_SmallValues()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(0.001, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(0.002, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var sum    = first.Add(second, decimalPlaces: 6); // preserve sub-unit precision

            Assert.AreEqual(0.003,  sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        // VolumeUnit enum constant tests

        [TestMethod]
        public void testVolumeUnitEnum_LitreConstant()
        {
            double factor = new VolumeUnitExtensions(VolumeUnit.LITRE).GetConversionFactor();
            Assert.AreEqual(1.0, factor, EPSILON);
        }

        [TestMethod]
        public void testVolumeUnitEnum_MillilitreConstant()
        {
            double factor = new VolumeUnitExtensions(VolumeUnit.MILLILITRE).GetConversionFactor();
            Assert.AreEqual(0.001, factor, EPSILON);
        }

        [TestMethod]
        public void testVolumeUnitEnum_GallonConstant()
        {
            double factor = new VolumeUnitExtensions(VolumeUnit.GALLON).GetConversionFactor();
            Assert.AreEqual(LitresPerGallon, factor, EPSILON);
        }

        // convertToBaseUnit tests

        [TestMethod]
        public void testConvertToBaseUnit_LitreToLitre()
        {
            double result = new VolumeUnitExtensions(VolumeUnit.LITRE).ConvertToBaseUnit(5.0);
            Assert.AreEqual(5.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_MillilitreToLitre()
        {
            double result = new VolumeUnitExtensions(VolumeUnit.MILLILITRE).ConvertToBaseUnit(1000.0);
            Assert.AreEqual(1.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertToBaseUnit_GallonToLitre()
        {
            double result = new VolumeUnitExtensions(VolumeUnit.GALLON).ConvertToBaseUnit(1.0);
            Assert.AreEqual(LitresPerGallon, result, EPSILON);
        }

        // convertFromBaseUnit tests

        [TestMethod]
        public void testConvertFromBaseUnit_LitreToLitre()
        {
            double result = new VolumeUnitExtensions(VolumeUnit.LITRE).ConvertFromBaseUnit(2.0);
            Assert.AreEqual(2.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_LitreToMillilitre()
        {
            double result = new VolumeUnitExtensions(VolumeUnit.MILLILITRE).ConvertFromBaseUnit(1.0);
            Assert.AreEqual(1000.0, result, EPSILON);
        }

        [TestMethod]
        public void testConvertFromBaseUnit_LitreToGallon()
        {
            double result = new VolumeUnitExtensions(VolumeUnit.GALLON).ConvertFromBaseUnit(LitresPerGallon);
            Assert.AreEqual(1.0, result, EPSILON);
        }

        // Backward compatibility and scalability tests

        [TestMethod]
        public void testBackwardCompatibility_AllUC1Through10Tests()
        {
            // UC1 — Feet equality
            var feet1 = new Feet(10);
            var feet2 = new Feet(10);
            Assert.AreEqual(feet1, feet2);

            // UC3 — Generic Length equality
            var length1 = new Length(1.0, LengthUnit.FEET);
            var length2 = new Length(12.0, LengthUnit.INCHES);
            Assert.IsTrue(length1.Equals(length2));

            // UC5 — Conversion
            double converted = Length.Convert(1.0, LengthUnit.FEET, LengthUnit.INCHES);
            Assert.AreEqual(12.0, converted, EPSILON);

            // UC6 — Addition
            var lengthSum = length1.Add(length2);
            Assert.AreEqual(2.0, lengthSum.Value, EPSILON);

            // UC9 — Weight equality
            var kg   = new Weight(1.0, WeightUnit.KILOGRAM);
            var gram = new Weight(1000.0, WeightUnit.GRAM);
            Assert.IsTrue(kg.Equals(gram));

            // UC9 — Weight conversion
            var kgConverted = kg.ConvertTo(WeightUnit.GRAM);
            Assert.AreEqual(1000.0, kgConverted.Value, EPSILON);

            // UC9 — Weight addition
            var weightSum = kg.Add(gram, WeightUnit.KILOGRAM);
            Assert.AreEqual(2.0, weightSum.Value, EPSILON);

            // UC10 — Generic Quantity Length
            var gFeet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var gInches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.IsTrue(gFeet.Equals(gInches));

            // UC10 — Generic Quantity Weight
            var gKg   = new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gGram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.IsTrue(gKg.Equals(gGram));
        }

        [TestMethod]
        public void testGenericQuantity_VolumeOperations_Consistency()
        {
            // Equality — same generic method that handles Length and Weight
            var litre      = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var millilitre = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.IsTrue(litre.Equals(millilitre));

            // Conversion — same generic method
            var converted = litre.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(1000.0, converted.Value, EPSILON);
            Assert.AreEqual("MILLILITRE", converted.Unit.GetUnitName());

            // Addition — same generic method
            var sum = litre.Add(millilitre, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(2.0,    sum.Value,           EPSILON);
            Assert.AreEqual("LITRE", sum.Unit.GetUnitName());
        }

        [TestMethod]
        public void testScalability_VolumeIntegration()
        {
            // Confirms Volume integrates with existing generic Quantity<TUnit> without
            // any changes to the class — architecture scales linearly with new categories.
            var volume = new Quantity<VolumeUnitMeasurable>(1.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var length = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            // All three use the same Quantity<TUnit> class — no new overloads or subclasses
            Assert.IsNotNull(volume);
            Assert.IsNotNull(length);
            Assert.IsNotNull(weight);

            // Runtime types are all distinct — compiler enforces category isolation
            Assert.AreNotEqual(volume.GetType(), length.GetType());
            Assert.AreNotEqual(volume.GetType(), weight.GetType());
            Assert.AreNotEqual(length.GetType(), weight.GetType());

            // Cross-category equality always false — runtime check works correctly
            Assert.IsFalse(volume.Equals(length));
            Assert.IsFalse(volume.Equals(weight));

            // Volume operations go through the same generic Quantity<TUnit> methods
            var converted = volume.ConvertTo(new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(1000.0, converted.Value, EPSILON);

            var ml  = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            var sum = volume.Add(ml, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(2.0, sum.Value, EPSILON);
        }
    }
}