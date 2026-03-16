using QuantityMeasurementBusinessLayer.Service;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Unit;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC13: Tests for the centralized arithmetic refactor inside Quantity&lt;TUnit&gt;.
    /// Contains exactly the 40 test cases specified for UC13.
    /// Framework: MSTest
    /// </summary>
    [TestClass]
    public class CentralizedArithmeticLogicTests
    {
        private const double EPSILON = 1e-4;

        // ─────────────────────────────────────────────────────────────────────
        // HELPER DELEGATION
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that Add() calls PerformBaseArithmetic with the ADD enum.
        /// Observed indirectly: result matches ADD semantics (sum), not difference or ratio.
        /// Tests: Helper delegation works correctly.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Add_DelegatesViaHelper()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var result = a.Add(b);
            // ADD enum: 10 + 5 = 15, not 5 (subtract) and not 2.0 (divide)
            Assert.AreEqual(15.0, result.Value, EPSILON);
            Assert.AreNotEqual(5.0, result.Value, EPSILON);
            Assert.AreNotEqual(2.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies that Subtract() calls PerformBaseArithmetic with the SUBTRACT enum.
        /// Observed indirectly: result matches SUBTRACT semantics (difference), not sum or ratio.
        /// Tests: Helper delegation works correctly.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Subtract_DelegatesViaHelper()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var result = a.Subtract(b);
            // SUBTRACT enum: 10 - 5 = 5, not 15 (add) and not 2.0 (divide)
            Assert.AreEqual(5.0,  result.Value, EPSILON);
            Assert.AreNotEqual(15.0, result.Value, EPSILON);
            Assert.AreNotEqual(2.0,  result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies that Divide() calls PerformBaseArithmetic with the DIVIDE enum.
        /// Observed indirectly: result matches DIVIDE semantics (ratio), not sum or difference.
        /// Tests: Helper delegation works correctly.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Divide_DelegatesViaHelper()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            double result = a.Divide(b);
            // DIVIDE enum: 10 / 5 = 2.0, not 15 (add) and not 5 (subtract)
            Assert.AreEqual(2.0,  result, EPSILON);
            Assert.AreNotEqual(15.0, result, EPSILON);
            Assert.AreNotEqual(5.0,  result, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // VALIDATION CONSISTENCY
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that Add(null), Subtract(null), Divide(null) all throw with the same
        /// exception type and param name — proves validation is centralized.
        /// Tests: Validation consistency.
        /// </summary>
        [TestMethod]
        public void testValidation_NullOperand_ConsistentAcrossOperations()
        {
            var q = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));

            var exAdd      = Assert.Throws<ArgumentNullException>(() => q.Add(null!));
            var exSubtract = Assert.Throws<ArgumentNullException>(() => q.Subtract(null!));
            var exDivide   = Assert.Throws<ArgumentNullException>(() => q.Divide(null!));

            // Same param name across all three — single validation point
            Assert.AreEqual(exAdd.ParamName, exSubtract.ParamName);
            Assert.AreEqual(exAdd.ParamName, exDivide.ParamName);
        }

        /// <summary>
        /// Verifies that cross-category checks work identically for all operations.
        /// Compile-time: distinct generic types prevent cross-category calls.
        /// Runtime: GetType() confirms no shared instantiation.
        /// Tests: Category validation centralization.
        /// </summary>
        [TestMethod]
        public void testValidation_CrossCategory_ConsistentAcrossOperations()
        {
            var length = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(5.0,  new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var volume = new Quantity<VolumeUnitMeasurable>(3.0,  new VolumeUnitMeasurable(VolumeUnit.LITRE));

            Assert.AreNotEqual(length.GetType(), weight.GetType());
            Assert.AreNotEqual(length.GetType(), volume.GetType());
            Assert.AreNotEqual(weight.GetType(), volume.GetType());

            Assert.IsFalse(length.Equals(weight));
            Assert.IsFalse(length.Equals(volume));
            Assert.IsFalse(weight.Equals(volume));
        }

        /// <summary>
        /// Verifies that NaN and Infinity values are rejected consistently.
        /// The constructor rejects non-finite values, so all operations fail identically.
        /// Tests: Finiteness validation consistency.
        /// </summary>
        [TestMethod]
        public void testValidation_FiniteValue_ConsistentAcrossOperations()
        {
            Assert.Throws<ArgumentException>(() =>
                new Quantity<LengthUnitMeasurable>(double.NaN,              new LengthUnitMeasurable(LengthUnit.FEET)));
            Assert.Throws<ArgumentException>(() =>
                new Quantity<LengthUnitMeasurable>(double.PositiveInfinity, new LengthUnitMeasurable(LengthUnit.FEET)));
            Assert.Throws<ArgumentException>(() =>
                new Quantity<LengthUnitMeasurable>(double.NegativeInfinity, new LengthUnitMeasurable(LengthUnit.FEET)));

            // A valid finite value passes
            var valid = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.IsNotNull(valid);
        }

        /// <summary>
        /// Verifies that an explicit null (default struct) target unit throws an exception
        /// for both Add and Subtract.
        /// Tests: Target unit validation for add/subtract.
        /// </summary>
        [TestMethod]
        public void testValidation_NullTargetUnit_AddSubtractReject()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));

            // default(LengthUnitMeasurable) wraps LengthUnit.UNKNOWN — rejected as an invalid target
            Assert.Throws<ArgumentException>(() => first.Add(second,      default(LengthUnitMeasurable)));
            Assert.Throws<ArgumentException>(() => first.Subtract(second, default(LengthUnitMeasurable)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // ENUM COMPUTATION
        // Tested via the public API since ArithmeticOperation is private.
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that the ADD enum constant computes the correct sum.
        /// Direct equivalent: ADD.compute(10, 5) == 15.0.
        /// Tests: Enum operation logic.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_Add_EnumComputation()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(15.0, a.Add(b).Value, EPSILON);
        }

        /// <summary>
        /// Verifies that the SUBTRACT enum constant computes the correct difference.
        /// Direct equivalent: SUBTRACT.compute(10, 5) == 5.0.
        /// Tests: Enum operation logic.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_Subtract_EnumComputation()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(5.0, a.Subtract(b).Value, EPSILON);
        }

        /// <summary>
        /// Verifies that the DIVIDE enum constant computes the correct quotient.
        /// Direct equivalent: DIVIDE.compute(10, 5) == 2.0.
        /// Tests: Enum operation logic.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_Divide_EnumComputation()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(2.0, a.Divide(b), EPSILON);
        }

        /// <summary>
        /// Verifies that the DIVIDE enum constant throws ArithmeticException when divisor is zero.
        /// Direct equivalent: DIVIDE.compute(10, 0) throws ArithmeticException.
        /// Tests: Enum-level validation.
        /// </summary>
        [TestMethod]
        public void testArithmeticOperation_DivideByZero_EnumThrows()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(0.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArithmeticException>(() => a.Divide(b));
        }

        /// <summary>
        /// Verifies that PerformBaseArithmetic converts both operands to the base unit
        /// before applying the operation.
        /// 1 ft + 12 in: base values are 1.0 ft + 1.0 ft = 2.0 ft (not raw 1 + 12 = 13).
        /// Tests: Helper correctness.
        /// </summary>
        [TestMethod]
        public void testPerformBaseArithmetic_ConversionAndOperation()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = feet.Add(inches);
            Assert.AreEqual(2.0,  result.Value, EPSILON); // correct: base-unit normalised
            Assert.AreNotEqual(13.0, result.Value, EPSILON); // wrong: raw value shortcut
        }

        // ─────────────────────────────────────────────────────────────────────
        // BACKWARD COMPATIBILITY
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Runs all UC12 addition test cases to confirm Add behaviour is unchanged.
        /// Tests: Backward compatibility.
        /// </summary>
        [TestMethod]
        public void testAdd_UC12_BehaviorPreserved()
        {
            // Same-unit: 1 ft + 1 ft = 2 ft
            var a = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(2.0, a.Add(b).Value, EPSILON);

            // Cross-unit: 1 ft + 12 in = 2 ft; explicit INCHES = 24 in
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(2.0,  feet.Add(inches).Value, EPSILON);
            Assert.AreEqual(24.0, feet.Add(inches, new LengthUnitMeasurable(LengthUnit.INCHES)).Value, EPSILON);

            // Weight: 1 kg + 1000 g = 2 kg
            var kg   = new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.AreEqual(2.0, kg.Add(gram).Value, EPSILON);

            // Volume: 1 L + 1000 mL = 2 L
            var litre = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var ml    = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));
            Assert.AreEqual(2.0, litre.Add(ml).Value, EPSILON);
        }

        /// <summary>
        /// Runs all UC12 subtraction test cases to confirm Subtract behaviour is unchanged.
        /// Tests: Backward compatibility.
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved()
        {
            // Same-unit: 10 ft - 5 ft = 5 ft
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(5.0, a.Subtract(b).Value, EPSILON);

            // Cross-unit: 10 ft - 6 in = 9.5 ft; explicit INCHES = 114 in
            var feet   = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(9.5,   feet.Subtract(inches).Value, EPSILON);
            Assert.AreEqual(114.0, feet.Subtract(inches, new LengthUnitMeasurable(LengthUnit.INCHES)).Value, EPSILON);

            // Negative result: 5 ft - 10 ft = -5 ft
            var x = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var y = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(-5.0, x.Subtract(y).Value, EPSILON);

            // Volume explicit target: 5 L - 2 L in MILLILITRE = 3000 mL
            var l1 = new Quantity<VolumeUnitMeasurable>(5.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var l2 = new Quantity<VolumeUnitMeasurable>(2.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(3000.0, l1.Subtract(l2, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE)).Value, EPSILON);
        }

        /// <summary>
        /// Runs all UC12 division test cases to confirm Divide behaviour is unchanged.
        /// Tests: Backward compatibility.
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved()
        {
            // Same-unit: 10 ft / 2 ft = 5.0
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(5.0, a.Divide(b), EPSILON);

            // Cross-unit: 24 in / 2 ft = 1.0
            var i24 = new Quantity<LengthUnitMeasurable>(24.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var f2  = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(1.0, i24.Divide(f2), EPSILON);

            // Weight: 2 kg / 2000 g = 1.0
            var kg   = new Quantity<WeightUnitMeasurable>(2.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(2000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.AreEqual(1.0, kg.Divide(gram), EPSILON);

            // Volume: 10 L / 5 L = 2.0
            var l1 = new Quantity<VolumeUnitMeasurable>(10.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var l2 = new Quantity<VolumeUnitMeasurable>(5.0,  new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(2.0, l1.Divide(l2), EPSILON);

            // Division by zero throws
            var zero = new Quantity<LengthUnitMeasurable>(0.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArithmeticException>(() => a.Divide(zero));
        }

        // ─────────────────────────────────────────────────────────────────────
        // ROUNDING
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that Add and Subtract results are rounded to two decimal places via BuildResult.
        /// Tests: Rounding consistency via helper.
        /// </summary>
        [TestMethod]
        public void testRounding_AddSubtract_TwoDecimalPlaces()
        {
            // 1 ft + 4 in = 1.333... ft → rounds to 1.33
            var feet   = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(4.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(1.33, feet.Add(inches).Value,      EPSILON);

            // 1 ft - 4 in = 0.666... ft → rounds to 0.67
            Assert.AreEqual(0.67, feet.Subtract(inches).Value, EPSILON);
        }

        /// <summary>
        /// Verifies that Divide returns a raw double without rounding.
        /// 10 / 3 = 3.333... — must be greater than the 2-dp rounded value 3.33.
        /// Tests: Different handling for dimensionless results.
        /// </summary>
        [TestMethod]
        public void testRounding_Divide_NoRounding()
        {
            var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(3.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            double ratio = a.Divide(b);
            Assert.IsTrue(ratio > 3.333 && ratio < 3.334,
                $"Expected raw double ≈ 3.3333 but got {ratio}");
        }

        // ─────────────────────────────────────────────────────────────────────
        // TARGET UNIT
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that Add and Subtract without an explicit target unit express the result
        /// in the first operand's unit (implicit target).
        /// Tests: Implicit target unit behavior.
        /// </summary>
        [TestMethod]
        public void testImplicitTargetUnit_AddSubtract()
        {
            // First operand FEET → result FEET
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual("FEET",   feet.Add(inches).Unit.GetUnitName());

            // First operand INCHES → result INCHES
            Assert.AreEqual("INCHES", inches.Add(feet).Unit.GetUnitName());

            // First operand KILOGRAM → result KILOGRAM
            var kg   = new Quantity<WeightUnitMeasurable>(5.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.AreEqual("KILOGRAM", kg.Subtract(gram).Unit.GetUnitName());
        }

        /// <summary>
        /// Verifies that an explicit target unit overrides the first operand's unit for both Add and Subtract.
        /// Tests: Explicit target unit behavior.
        /// </summary>
        [TestMethod]
        public void testExplicitTargetUnit_AddSubtract_Overrides()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

            // Add: first operand is FEET but explicit target is YARDS
            var addResult = feet.Add(inches, new LengthUnitMeasurable(LengthUnit.YARDS));
            Assert.AreEqual("YARDS", addResult.Unit.GetUnitName());
            Assert.AreEqual(0.67, addResult.Value, EPSILON); // (1 ft + 1 ft) in yards = 0.6667

            // Subtract: first operand is FEET but explicit target is INCHES
            var subtractResult = feet.Subtract(
                new Quantity<LengthUnitMeasurable>(6.0, new LengthUnitMeasurable(LengthUnit.INCHES)),
                new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual("INCHES", subtractResult.Unit.GetUnitName());
            Assert.AreEqual(6.0, subtractResult.Value, EPSILON); // 12 in - 6 in = 6 in
        }

        // ─────────────────────────────────────────────────────────────────────
        // IMMUTABILITY — each operation tested individually
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that original quantities are unchanged after addition through the centralized helper.
        /// Tests: Immutability through refactored implementation.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterAdd_ViaCentralizedHelper()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(4.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            _ = first.Add(second);
            Assert.AreEqual(10.0, first.Value,  EPSILON);
            Assert.AreEqual(4.0,  second.Value, EPSILON);
        }

        /// <summary>
        /// Verifies that original quantities are unchanged after subtraction through the centralized helper.
        /// Tests: Immutability through refactored implementation.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterSubtract_ViaCentralizedHelper()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(4.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            _ = first.Subtract(second);
            Assert.AreEqual(10.0, first.Value,  EPSILON);
            Assert.AreEqual(4.0,  second.Value, EPSILON);
        }

        /// <summary>
        /// Verifies that original quantities are unchanged after division through the centralized helper.
        /// Tests: Immutability through refactored implementation.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterDivide_ViaCentralizedHelper()
        {
            var first  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(4.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            _ = first.Divide(second);
            Assert.AreEqual(10.0, first.Value,  EPSILON);
            Assert.AreEqual(4.0,  second.Value, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // ALL CATEGORIES
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies Add, Subtract, and Divide all work correctly across Length, Weight, and Volume.
        /// Tests: Scalability across categories.
        /// </summary>
        [TestMethod]
        public void testAllOperations_AcrossAllCategories()
        {
            // Length
            var lA = new Quantity<LengthUnitMeasurable>(9.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var lB = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(12.0, lA.Add(lB).Value,      EPSILON);
            Assert.AreEqual(6.0,  lA.Subtract(lB).Value, EPSILON);
            Assert.AreEqual(3.0,  lA.Divide(lB),          EPSILON);

            // Weight
            var wA = new Quantity<WeightUnitMeasurable>(9.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var wB = new Quantity<WeightUnitMeasurable>(3.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(12.0, wA.Add(wB).Value,      EPSILON);
            Assert.AreEqual(6.0,  wA.Subtract(wB).Value, EPSILON);
            Assert.AreEqual(3.0,  wA.Divide(wB),          EPSILON);

            // Volume
            var vA = new Quantity<VolumeUnitMeasurable>(9.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var vB = new Quantity<VolumeUnitMeasurable>(3.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(12.0, vA.Add(vB).Value,      EPSILON);
            Assert.AreEqual(6.0,  vA.Subtract(vB).Value, EPSILON);
            Assert.AreEqual(3.0,  vA.Divide(vB),          EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DRY PRINCIPLE
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Confirms validation logic is centralized: same exception type and param name
        /// across all three operations, proving validation is not duplicated.
        /// Tests: DRY principle enforcement.
        /// </summary>
        [TestMethod]
        public void testCodeDuplication_ValidationLogic_Eliminated()
        {
            var q = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));

            var exAdd      = Assert.Throws<ArgumentNullException>(() => q.Add(null!));
            var exSubtract = Assert.Throws<ArgumentNullException>(() => q.Subtract(null!));
            var exDivide   = Assert.Throws<ArgumentNullException>(() => q.Divide(null!));

            // Single validation point — param name is identical across all operations
            Assert.AreEqual(exAdd.ParamName,      exSubtract.ParamName);
            Assert.AreEqual(exSubtract.ParamName, exDivide.ParamName);
        }

        /// <summary>
        /// Confirms conversion logic is centralized: cross-unit operations produce correct
        /// base-unit-normalised results regardless of which public method is called.
        /// Tests: Centralization of conversion.
        /// </summary>
        [TestMethod]
        public void testCodeDuplication_ConversionLogic_Eliminated()
        {
            // 1 ft and 12 in are the same physical length
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

            // All three use the same PerformBaseArithmetic conversion path
            Assert.AreEqual(2.0, feet.Add(inches).Value,      EPSILON); // 1 ft + 1 ft
            Assert.AreEqual(0.0, feet.Subtract(inches).Value, EPSILON); // 1 ft - 1 ft
            Assert.AreEqual(1.0, feet.Divide(inches),          EPSILON); // 1 ft / 1 ft
        }

        // ─────────────────────────────────────────────────────────────────────
        // ENUM DISPATCH CORRECTNESS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies each operation uses the correct enum constant by checking all three
        /// produce the expected distinct result for the same input pair.
        /// Tests: Enum-based dispatch correctness.
        /// </summary>
        [TestMethod]
        public void testEnumDispatch_AllOperations_CorrectlyDispatched()
        {
            var a = new Quantity<WeightUnitMeasurable>(12.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var b = new Quantity<WeightUnitMeasurable>(4.0,  new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            Assert.AreEqual(16.0, a.Add(b).Value,      EPSILON); // ADD:      12 + 4
            Assert.AreEqual(8.0,  a.Subtract(b).Value, EPSILON); // SUBTRACT: 12 - 4
            Assert.AreEqual(3.0,  a.Divide(b),          EPSILON); // DIVIDE:   12 / 4

            // All three results are distinct — confirms each enum branch was reached
            Assert.AreNotEqual(a.Add(b).Value,      a.Subtract(b).Value, EPSILON);
            Assert.AreNotEqual(a.Add(b).Value,      a.Divide(b),          EPSILON);
            Assert.AreNotEqual(a.Subtract(b).Value, a.Divide(b),          EPSILON);
        }

        /// <summary>
        /// Tests the hypothetical MULTIPLY operation pattern using the same base-unit approach.
        /// 3 ft × 4 ft: base 3.0 × base 4.0 = 12.0 — confirms the pattern would work.
        /// Tests: Scalability for future operations.
        /// </summary>
        [TestMethod]
        public void testFutureOperation_MultiplicationPattern()
        {
            var a = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(4.0, new LengthUnitMeasurable(LengthUnit.FEET));

            double baseA    = a.ToBaseUnit(); // 3.0
            double baseB    = b.ToBaseUnit(); // 4.0
            double product  = baseA * baseB;  // 12.0 — same pattern PerformBaseArithmetic uses

            Assert.AreEqual(3.0,  baseA,   EPSILON);
            Assert.AreEqual(4.0,  baseB,   EPSILON);
            Assert.AreEqual(12.0, product, EPSILON);
            // A future MULTIPLY enum case would add: ArithmeticOperation.Multiply => a * b
        }

        // ─────────────────────────────────────────────────────────────────────
        // ERROR MESSAGE CONSISTENCY
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Compares error messages from Add, Subtract, and Divide for the same error type.
        /// All originate from the same validation point so the param name must be identical.
        /// Tests: Consistent error reporting.
        /// </summary>
        [TestMethod]
        public void testErrorMessage_Consistency_Across_Operations()
        {
            var q = new Quantity<VolumeUnitMeasurable>(5.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));

            var exAdd      = Assert.Throws<ArgumentNullException>(() => q.Add(null!));
            var exSubtract = Assert.Throws<ArgumentNullException>(() => q.Subtract(null!));
            var exDivide   = Assert.Throws<ArgumentNullException>(() => q.Divide(null!));

            Assert.AreEqual(exAdd.ParamName,      exSubtract.ParamName);
            Assert.AreEqual(exSubtract.ParamName, exDivide.ParamName);

            Assert.IsTrue(exAdd.Message.Contains(exAdd.ParamName!));
            Assert.IsTrue(exSubtract.Message.Contains(exSubtract.ParamName!));
            Assert.IsTrue(exDivide.Message.Contains(exDivide.ParamName!));
        }

        // ─────────────────────────────────────────────────────────────────────
        // ENCAPSULATION
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that PerformBaseArithmetic is private — not visible as a public method.
        /// Tests: Encapsulation.
        /// </summary>
        [TestMethod]
        public void testHelper_PrivateVisibility()
        {
            var type       = typeof(Quantity<LengthUnitMeasurable>);
            var publicMethods = type.GetMethods(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var method in publicMethods)
                Assert.AreNotEqual("PerformBaseArithmetic", method.Name,
                    "PerformBaseArithmetic must be private, not public");
        }

        /// <summary>
        /// Verifies that ValidateArithmeticOperands is private — not visible as a public method.
        /// Tests: Encapsulation.
        /// </summary>
        [TestMethod]
        public void testValidation_Helper_PrivateVisibility()
        {
            var type       = typeof(Quantity<LengthUnitMeasurable>);
            var publicMethods = type.GetMethods(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var method in publicMethods)
                Assert.AreNotEqual("ValidateArithmeticOperands", method.Name,
                    "ValidateArithmeticOperands must be private, not public");
        }

        // ─────────────────────────────────────────────────────────────────────
        // ROUNDING HELPER ACCURACY
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that BuildResult rounds correctly: a raw value of 1.333... rounds to 1.33,
        /// not 1.334 or 1.3333.
        /// Tests: Rounding helper correctness.
        /// </summary>
        [TestMethod]
        public void testRounding_Helper_Accuracy()
        {
            // 1 ft + 4 in = 1.333... ft → BuildResult rounds to 1.33
            var feet   = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(4.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            double result = feet.Add(inches).Value;
            Assert.AreEqual(1.33, result, EPSILON);
            Assert.AreNotEqual(1.334,  result, EPSILON);
            Assert.AreNotEqual(1.3333, result, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // OPERATION CHAINING
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies q1.Add(q2).Subtract(q3).Divide(q4) chains correctly through the refactored methods.
        /// (6 + 3 - 3) / 3 = 6 / 3 = 2.0.
        /// Tests: Operation composition through refactored methods.
        /// </summary>
        [TestMethod]
        public void testArithmetic_Chain_Operations()
        {
            var q1 = new Quantity<LengthUnitMeasurable>(6.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var q2 = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var q3 = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var q4 = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            double result = q1.Add(q2).Subtract(q3).Divide(q4); // (6+3-3)/3 = 2.0
            Assert.AreEqual(2.0, result, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // LARGE DATASET & PERFORMANCE
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Runs 1000+ Add, Subtract, and Divide operations to confirm no regressions from UC13 refactor.
        /// Tests: Behavioral equivalence at scale.
        /// </summary>
        [TestMethod]
        public void testRefactoring_NoBehaviorChange_LargeDataset()
        {
            for (int i = 1; i <= 1000; i++)
            {
                double d = (double)i;
                var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
                var b = new Quantity<LengthUnitMeasurable>(d,    new LengthUnitMeasurable(LengthUnit.FEET));

                Assert.AreEqual(Math.Round(10.0 + d, 2), a.Add(b).Value,      EPSILON, $"Add failed at i={i}");
                Assert.AreEqual(Math.Round(10.0 - d, 2), a.Subtract(b).Value, EPSILON, $"Subtract failed at i={i}");
                Assert.AreEqual(10.0 / d,                a.Divide(b),          EPSILON, $"Divide failed at i={i}");
            }
        }

        /// <summary>
        /// Confirms no performance regression: 10,000 operations complete within 5 seconds.
        /// Tests: No performance regression from refactoring.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Performance_ComparableToUC12()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 1; i <= 10_000; i++)
            {
                var a = new Quantity<LengthUnitMeasurable>(10.0,    new LengthUnitMeasurable(LengthUnit.FEET));
                var b = new Quantity<LengthUnitMeasurable>((double)i, new LengthUnitMeasurable(LengthUnit.FEET));
                _ = a.Add(b);
                _ = a.Subtract(b);
                _ = a.Divide(b);
            }

            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds < 5000,
                $"10,000 operations took {sw.ElapsedMilliseconds}ms — expected under 5000ms");
        }

        // ─────────────────────────────────────────────────────────────────────
        // DIRECT ENUM CONSTANT TESTS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Direct test: ADD enum computes 7 + 3 = 10.0.
        /// </summary>
        [TestMethod]
        public void testEnumConstant_ADD_CorrectlyAdds()
        {
            var a = new Quantity<LengthUnitMeasurable>(7.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(10.0, a.Add(b).Value, EPSILON);
        }

        /// <summary>
        /// Direct test: SUBTRACT enum computes 7 - 3 = 4.0.
        /// </summary>
        [TestMethod]
        public void testEnumConstant_SUBTRACT_CorrectlySubtracts()
        {
            var a = new Quantity<LengthUnitMeasurable>(7.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(4.0, a.Subtract(b).Value, EPSILON);
        }

        /// <summary>
        /// Direct test: DIVIDE enum computes 7 / 2 = 3.5.
        /// </summary>
        [TestMethod]
        public void testEnumConstant_DIVIDE_CorrectlyDivides()
        {
            var a = new Quantity<LengthUnitMeasurable>(7.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(2.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(3.5, a.Divide(b), EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // BASE UNIT CONVERSION
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies PerformBaseArithmetic converts both operands to base unit before operating.
        /// If raw values were used, 1 ft + 12 in = 13 (wrong). Correct answer is 2.
        /// Tests: Helper converts to base unit before operation.
        /// </summary>
        [TestMethod]
        public void testHelper_BaseUnitConversion_Correct()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = feet.Add(inches);
            Assert.AreEqual(2.0,   result.Value, EPSILON); // correct: base-unit normalised
            Assert.AreNotEqual(13.0, result.Value, EPSILON); // wrong: raw value shortcut
        }

        /// <summary>
        /// Verifies BuildResult converts the raw base-unit result back to the target unit.
        /// 1 ft + 12 in = 2 ft base → expressed in INCHES = 24 in.
        /// Tests: Result is correctly converted from base unit to target unit.
        /// </summary>
        [TestMethod]
        public void testHelper_ResultConversion_Correct()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var result = feet.Add(inches, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(24.0,     result.Value,             EPSILON); // 2 ft base → 24 in
            Assert.AreEqual("INCHES", result.Unit.GetUnitName());
        }

        // ─────────────────────────────────────────────────────────────────────
        // UNIFIED VALIDATION
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies all three operations reject the same invalid inputs with consistent
        /// exception types and parameter names — proving validation is unified in one place.
        /// Tests: Consistent error reporting.
        /// </summary>
        [TestMethod]
        public void testRefactoring_Validation_UnifiedBehavior()
        {
            var valid  = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var second = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));

            // All three reject null with ArgumentNullException and the same param name
            var exAdd      = Assert.Throws<ArgumentNullException>(() => valid.Add(null!));
            var exSubtract = Assert.Throws<ArgumentNullException>(() => valid.Subtract(null!));
            var exDivide   = Assert.Throws<ArgumentNullException>(() => valid.Divide(null!));
            Assert.AreEqual(exAdd.ParamName, exSubtract.ParamName);
            Assert.AreEqual(exAdd.ParamName, exDivide.ParamName);

            // Add and Subtract both reject an invalid target unit
            Assert.Throws<ArgumentException>(() => valid.Add(second,      default(LengthUnitMeasurable)));
            Assert.Throws<ArgumentException>(() => valid.Subtract(second, default(LengthUnitMeasurable)));

            // Divide-by-zero is caught inside the same helper
            var zero = new Quantity<LengthUnitMeasurable>(0.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.Throws<ArithmeticException>(() => valid.Divide(zero));
        }
    }
}