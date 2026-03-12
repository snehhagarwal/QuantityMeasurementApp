using QuantityMeasurementBusinessLayer.Service;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC12: Tests for Subtraction and Division operations on Quantity&lt;TUnit&gt;.
    /// Subtraction returns a new Quantity in the specified (or implicit) target unit.
    /// Division returns a dimensionless double ratio.
    /// Framework: MSTest
    /// </summary>
    [TestClass]
    public class SubtractionAndDivisionTests
    {
        private const double EPSILON = 1e-4;

        // ═══════════════════════════════════════════════════════════════════════
        // SUBTRACTION TESTS
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies that 10.0 FEET - 5.0 FEET = 5.0 FEET.
        /// Tests: Same-unit subtraction without conversion.
        /// </summary>
        [TestMethod]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var result = first.Subtract(second);
            Assert.AreEqual(5.0,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 10.0 LITRE - 3.0 LITRE = 7.0 LITRE.
        /// Tests: Same-unit subtraction for volume.
        /// </summary>
        [TestMethod]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(10.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(3.0,  new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result = first.Subtract(second);
            Assert.AreEqual(7.0,     result.Value,            EPSILON);
            Assert.AreEqual("LITRE", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 10.0 FEET - 6.0 INCHES = 9.5 FEET.
        /// Tests: Cross-unit subtraction with result in feet.
        /// </summary>
        [TestMethod]
        public void testSubtraction_CrossUnit_FeetMinusInches()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = first.Subtract(second);
            Assert.AreEqual(9.5,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 120.0 INCHES - 5.0 FEET = 60.0 INCHES.
        /// Tests: Cross-unit subtraction with result in inches.
        /// </summary>
        [TestMethod]
        public void testSubtraction_CrossUnit_InchesMinusFeet()
        {
            // 120 in - 5 ft = 120 in - 60 in = 60 in
            var first  = new Quantity<LengthUnitMeasurable>(120.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var second = new Quantity<LengthUnitMeasurable>(5.0,   new LengthUnitMeasurable(LengthUnit.FEET));
            var result = first.Subtract(second);
            Assert.AreEqual(60.0,     result.Value,             EPSILON);
            Assert.AreEqual("INCHES", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 10.0 FEET - 6.0 INCHES with explicit target FEET = 9.5 FEET.
        /// Tests: Explicit target unit specification in feet.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Feet()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = first.Subtract(second, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(9.5,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 10.0 FEET - 6.0 INCHES with explicit target INCHES = 114.0 INCHES.
        /// Tests: Explicit target unit specification in inches.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            // 10 ft = 120 in; 120 in - 6 in = 114 in
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = first.Subtract(second, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(114.0,    result.Value,             EPSILON);
            Assert.AreEqual("INCHES", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 5.0 LITRE - 2.0 LITRE with explicit target MILLILITRE = 3000.0 MILLILITRE.
        /// Tests: Explicit target unit specification in millilitre.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(5.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(2.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var result = first.Subtract(second, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(3000.0,       result.Value,              EPSILON);
            Assert.AreEqual("MILLILITRE", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 5.0 FEET - 10.0 FEET = -5.0 FEET.
        /// Tests: Subtraction resulting in negative values.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ResultingInNegative()
        {
            var first  = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var result = first.Subtract(second);
            Assert.AreEqual(-5.0,   result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 10.0 FEET - 120.0 INCHES = 0.0 FEET.
        /// Tests: Subtraction resulting in zero.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ResultingInZero()
        {
            // 120 in = 10 ft; 10 ft - 10 ft = 0 ft
            var first  = new Quantity<LengthUnitMeasurable>(10.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(120.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = first.Subtract(second);
            Assert.AreEqual(0.0,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 5.0 FEET - 0.0 INCHES = 5.0 FEET.
        /// Tests: Identity element property.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithZeroOperand()
        {
            var first  = new Quantity<LengthUnitMeasurable>(5.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(0.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = first.Subtract(second);
            Assert.AreEqual(5.0,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 5.0 FEET - (-2.0 FEET) = 7.0 FEET.
        /// Subtracting a negative is equivalent to adding its absolute value.
        /// Tests: Subtraction with negative operands.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithNegativeValues()
        {
            var first  = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(-2.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var result = first.Subtract(second);
            Assert.AreEqual(7.0,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that A.Subtract(B) != B.Subtract(A).
        /// 10 FEET - 5 FEET = 5.0; 5 FEET - 10 FEET = -5.0.
        /// Tests: Non-commutativity.
        /// </summary>
        [TestMethod]
        public void testSubtraction_NonCommutative()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(5.0,  a.Subtract(b).Value, EPSILON);  // A - B =  5.0
            Assert.AreEqual(-5.0, b.Subtract(a).Value, EPSILON);  // B - A = -5.0
        }

        /// <summary>
        /// Verifies that 1e6 KILOGRAM - 5e5 KILOGRAM = 5e5 KILOGRAM.
        /// Tests: Large magnitude subtraction.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithLargeValues()
        {
            var first  = new Quantity<WeightUnitMeasurable>(1_000_000.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var second = new Quantity<WeightUnitMeasurable>(500_000.0,   new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var result = first.Subtract(second);
            Assert.AreEqual(500_000.0,  result.Value,           EPSILON);
            Assert.AreEqual("KILOGRAM", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that 0.001 FEET - 0.0005 FEET = ~0.0005 FEET.
        /// Tests: Small magnitude subtraction with precision.
        /// </summary>
        [TestMethod]
        public void testSubtraction_WithSmallValues()
        {
            // Use higher decimalPlaces to preserve sub-centimetre precision
            var first  = new Quantity<LengthUnitMeasurable>(0.001,  new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(0.0005, new LengthUnitMeasurable(LengthUnit.FEET));
            var result = first.Subtract(second, decimalPlaces: 6);
            Assert.AreEqual(0.0005, result.Value, 1e-6);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that subtracting null throws ArgumentNullException.
        /// Tests: Null operand validation.
        /// </summary>
        [TestMethod]
        public void testSubtraction_NullOperand()
        {
            var first = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArgumentNullException>(() => first.Subtract(null!));
        }

        /// <summary>
        /// Verifies that a null target unit throws ArgumentException.
        /// LengthUnitMeasurable is a struct, so default(LengthUnitMeasurable) wraps
        /// LengthUnit.UNKNOWN (value 0) which ConvertFromBaseUnit rejects.
        /// Tests: Null target unit validation.
        /// </summary>
        [TestMethod]
        public void testSubtraction_NullTargetUnit()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArgumentException>(() =>
                first.Subtract(second, default(LengthUnitMeasurable)));
        }

        /// <summary>
        /// Verifies that cross-category subtraction (FEET vs KILOGRAM) is prevented
        /// by the generic type system at compile time.
        /// Runtime: confirmed via distinct GetType() values and false equality.
        /// Tests: Cross-category prevention.
        /// </summary>
        [TestMethod]
        public void testSubtraction_CrossCategory()
        {
            var length = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(5.0,  new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            // Compiler prevents: length.Subtract(weight) — different generic type arguments
            Assert.AreNotEqual(length.GetType(), weight.GetType());
            Assert.IsFalse(length.Equals(weight));
        }

        /// <summary>
        /// Verifies subtraction works correctly for Length, Weight, and Volume.
        /// Tests: Scalability across all measurement categories.
        /// </summary>
        [TestMethod]
        public void testSubtraction_AllMeasurementCategories()
        {
            // Length: 10 ft - 4 ft = 6 ft
            var lFirst  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var lSecond = new Quantity<LengthUnitMeasurable>(4.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(6.0, lFirst.Subtract(lSecond).Value, EPSILON);

            // Weight: 10 kg - 3 kg = 7 kg
            var wFirst  = new Quantity<WeightUnitMeasurable>(10.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var wSecond = new Quantity<WeightUnitMeasurable>(3.0,  new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(7.0, wFirst.Subtract(wSecond).Value, EPSILON);

            // Volume: 10 L - 4 L = 6 L
            var vFirst  = new Quantity<VolumeUnitMeasurable>(10.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var vSecond = new Quantity<VolumeUnitMeasurable>(4.0,  new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(6.0, vFirst.Subtract(vSecond).Value, EPSILON);
        }

        /// <summary>
        /// Verifies that 10 FEET.Subtract(2 FEET).Subtract(1 FEET) = 7.0 FEET.
        /// Tests: Method chaining support.
        /// </summary>
        [TestMethod]
        public void testSubtraction_ChainedOperations()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var c = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var result = a.Subtract(b).Subtract(c); // 10 - 2 - 1 = 7
            Assert.AreEqual(7.0,    result.Value,           EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        // ═══════════════════════════════════════════════════════════════════════
        // DIVISION TESTS
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies that 10.0 FEET / 2.0 FEET = 5.0.
        /// Tests: Same-unit division without conversion.
        /// </summary>
        [TestMethod]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(5.0, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that 10.0 LITRE / 5.0 LITRE = 2.0.
        /// Tests: Same-unit division for volume.
        /// </summary>
        [TestMethod]
        public void testDivision_SameUnit_LitreDividedByLitre()
        {
            var first  = new Quantity<VolumeUnitMeasurable>(10.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var second = new Quantity<VolumeUnitMeasurable>(5.0,  new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(2.0, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that 24.0 INCHES / 2.0 FEET = 1.0.
        /// 24 in = 2 ft, so ratio = 2 ft / 2 ft = 1.0.
        /// Tests: Cross-unit division with correct conversion.
        /// </summary>
        [TestMethod]
        public void testDivision_CrossUnit_FeetDividedByInches()
        {
            var first  = new Quantity<LengthUnitMeasurable>(24.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var second = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(1.0, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that 2.0 KILOGRAM / 2000.0 GRAM = 1.0.
        /// 2000 g = 2 kg, so ratio = 2 kg / 2 kg = 1.0.
        /// Tests: Cross-unit division for weight.
        /// </summary>
        [TestMethod]
        public void testDivision_CrossUnit_KilogramDividedByGram()
        {
            var first  = new Quantity<WeightUnitMeasurable>(2.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var second = new Quantity<WeightUnitMeasurable>(2000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.AreEqual(1.0, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that 10.0 FEET / 2.0 FEET = 5.0 (ratio > 1.0).
        /// Tests: Ratio > 1.0 case.
        /// </summary>
        [TestMethod]
        public void testDivision_RatioGreaterThanOne()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(5.0, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that 5.0 FEET / 10.0 FEET = 0.5 (ratio &lt; 1.0).
        /// Tests: Ratio &lt; 1.0 case.
        /// </summary>
        [TestMethod]
        public void testDivision_RatioLessThanOne()
        {
            var first  = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(0.5, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that 10.0 FEET / 10.0 FEET = 1.0 (ratio = 1.0).
        /// Tests: Equivalence detection through division.
        /// </summary>
        [TestMethod]
        public void testDivision_RatioEqualToOne()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(1.0, first.Divide(second), EPSILON);
        }

        /// <summary>
        /// Verifies that A.Divide(B) != B.Divide(A).
        /// 10 FEET / 5 FEET = 2.0; 5 FEET / 10 FEET = 0.5.
        /// Tests: Non-commutativity.
        /// </summary>
        [TestMethod]
        public void testDivision_NonCommutative()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(2.0, a.Divide(b), EPSILON); // A / B = 2.0
            Assert.AreEqual(0.5, b.Divide(a), EPSILON); // B / A = 0.5
        }

        /// <summary>
        /// Verifies that 10.0 FEET / 0.0 FEET throws ArithmeticException.
        /// Tests: Division by zero prevention.
        /// </summary>
        [TestMethod]
        public void testDivision_ByZero()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(0.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArithmeticException>(() => first.Divide(second));
        }

        /// <summary>
        /// Verifies that 1e6 KILOGRAM / 1.0 KILOGRAM = 1e6.
        /// Tests: Very large ratios.
        /// </summary>
        [TestMethod]
        public void testDivision_WithLargeRatio()
        {
            var first  = new Quantity<WeightUnitMeasurable>(1_000_000.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var second = new Quantity<WeightUnitMeasurable>(1.0,         new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(1_000_000.0, first.Divide(second), 1.0);
        }

        /// <summary>
        /// Verifies that 1.0 KILOGRAM / 1e6 KILOGRAM = 1e-6.
        /// Tests: Very small ratios and precision.
        /// </summary>
        [TestMethod]
        public void testDivision_WithSmallRatio()
        {
            var first  = new Quantity<WeightUnitMeasurable>(1.0,         new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var second = new Quantity<WeightUnitMeasurable>(1_000_000.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(1e-6, first.Divide(second), 1e-10);
        }

        /// <summary>
        /// Verifies that dividing by null throws ArgumentNullException.
        /// Tests: Null operand validation.
        /// </summary>
        [TestMethod]
        public void testDivision_NullOperand()
        {
            var first = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArgumentNullException>(() => first.Divide(null!));
        }

        /// <summary>
        /// Verifies that cross-category division (FEET vs KILOGRAM) is prevented
        /// by the generic type system at compile time.
        /// Runtime: confirmed via distinct GetType() values and false equality.
        /// Tests: Cross-category prevention.
        /// </summary>
        [TestMethod]
        public void testDivision_CrossCategory()
        {
            var length = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(5.0,  new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            // Compiler prevents: length.Divide(weight) — different generic type arguments
            Assert.AreNotEqual(length.GetType(), weight.GetType());
            Assert.IsFalse(length.Equals(weight));
        }

        /// <summary>
        /// Verifies division works correctly for Length, Weight, and Volume.
        /// Tests: Scalability across all measurement categories.
        /// </summary>
        [TestMethod]
        public void testDivision_AllMeasurementCategories()
        {
            // Length: 10 ft / 5 ft = 2.0
            var lFirst  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var lSecond = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(2.0, lFirst.Divide(lSecond), EPSILON);

            // Weight: 10 kg / 2 kg = 5.0
            var wFirst  = new Quantity<WeightUnitMeasurable>(10.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var wSecond = new Quantity<WeightUnitMeasurable>(2.0,  new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(5.0, wFirst.Divide(wSecond), EPSILON);

            // Volume: 9 L / 3 L = 3.0
            var vFirst  = new Quantity<VolumeUnitMeasurable>(9.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var vSecond = new Quantity<VolumeUnitMeasurable>(3.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(3.0, vFirst.Divide(vSecond), EPSILON);
        }

        /// <summary>
        /// Verifies that (A / B) / C != A / (B / C) — division is non-associative.
        /// A = 12, B = 6, C = 2 (all FEET).
        /// (A / B) / C = (12/6) / 2 = 2.0 / 2 = 1.0
        /// A / (B / C) = 12 / (6/2) = 12 / 3.0 = 4.0
        /// Tests: Mathematical property awareness.
        /// </summary>
        [TestMethod]
        public void testDivision_Associativity()
        {
            var a = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var c = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));

            double leftAssoc  = a.Divide(b) / c.Value;  // (A/B)/C = 1.0
            double rightAssoc = a.Value / b.Divide(c);   // A/(B/C) = 4.0

            Assert.AreNotEqual(leftAssoc, rightAssoc, EPSILON);
            Assert.AreEqual(1.0, leftAssoc,  EPSILON);
            Assert.AreEqual(4.0, rightAssoc, EPSILON);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // INTEGRATION TESTS
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies that subtraction and division coexist:
        /// A.Subtract(B).Divide(C) is valid and produces the correct result.
        /// (10 ft - 4 ft) / 3 ft = 6 ft / 3 ft = 2.0.
        /// Tests: Operation integration.
        /// </summary>
        [TestMethod]
        public void testSubtractionAndDivision_Integration()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(4.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var c = new Quantity<LengthUnitMeasurable>(3.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            double result = a.Subtract(b).Divide(c);
            Assert.AreEqual(2.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies that A.Add(B).Subtract(B) approximately equals A.
        /// Tests: Mathematical inverse relationship.
        /// </summary>
        [TestMethod]
        public void testSubtractionAddition_Inverse()
        {
            var a = new Quantity<LengthUnitMeasurable>(7.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var result = a.Add(b).Subtract(b); // (7 + 3) - 3 = 7
            Assert.AreEqual(a.Value, result.Value, EPSILON);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // IMMUTABILITY TESTS
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies that original quantities are unchanged after subtraction.
        /// Tests: Immutability principle.
        /// </summary>
        [TestMethod]
        public void testSubtraction_Immutability()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(4.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var result = first.Subtract(second);
            Assert.AreEqual(10.0, first.Value,  EPSILON); // first unchanged
            Assert.AreEqual(4.0,  second.Value, EPSILON); // second unchanged
            Assert.AreEqual(6.0,  result.Value, EPSILON); // new object holds difference
        }

        /// <summary>
        /// Verifies that original quantities are unchanged after division.
        /// Tests: Immutability principle.
        /// </summary>
        [TestMethod]
        public void testDivision_Immutability()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            double ratio = first.Divide(second);
            Assert.AreEqual(10.0, first.Value,  EPSILON); // first unchanged
            Assert.AreEqual(2.0,  second.Value, EPSILON); // second unchanged
            Assert.AreEqual(5.0,  ratio,         EPSILON); // result is a scalar
        }

        // ═══════════════════════════════════════════════════════════════════════
        // PRECISION TESTS
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies that subtraction results are rounded to two decimal places by default.
        /// 1.0 ft - 4.0 in: 12 in - 4 in = 8 in = 0.6667 ft → rounds to 0.67.
        /// Tests: Precision consistency.
        /// </summary>
        [TestMethod]
        public void testSubtraction_PrecisionAndRounding()
        {
            var first  = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(4.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = first.Subtract(second); // default decimalPlaces = 2
            // 0.6667 rounded to 2 decimal places = 0.67
            Assert.AreEqual(0.67, result.Value, EPSILON);
            Assert.AreEqual("FEET", result.Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that division results maintain floating-point precision (no rounding).
        /// 10 ft / 3 ft = 3.3333... — raw double, not rounded to 2 decimal places.
        /// Tests: Dimensionless result handling.
        /// </summary>
        [TestMethod]
        public void testDivision_PrecisionHandling()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(3.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            double ratio = first.Divide(second);
            // Raw double — precision beyond 2 decimal places is preserved
            Assert.IsTrue(ratio > 3.333 && ratio < 3.334,
                $"Expected ratio ≈ 3.3333 but got {ratio}");
        }
    }
}