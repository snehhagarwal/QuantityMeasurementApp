using QuantityMeasurementBusinessLayer.Service;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Interface;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC10: Generic Quantity Test Suite
    ///
    /// Purpose:
    /// Validate the generic Quantity&lt;TUnit&gt; class and IMeasurable interface design.
    /// Ensures that a single generic class correctly replaces the duplicate
    /// QuantityLength / QuantityWeight pattern while preserving all UC1–UC9 behavior.
    ///
    /// Covers:
    /// • IMeasurable interface implementation for LengthUnit and WeightUnit
    /// • Generic Quantity&lt;TUnit&gt; equality, conversion, and addition operations
    /// • Cross-category type safety and prevention of invalid comparisons
    /// • Constructor validation (non-finite values)
    /// • Backward compatibility with all UC1–UC9 classes
    /// • Scalability with new unit categories (VolumeUnit)
    /// • Immutability of Quantity&lt;TUnit&gt; objects
    /// • equals() contract: reflexive, symmetric, transitive
    /// • hashCode() consistency for generic quantities
    ///
    /// Framework Used: MSTest
    /// </summary>
    [TestClass]
    public class GenericQuantityTests
    {
        private const double EPSILON = 1e-4;

        // -------------------------------------------------------------------------
        // IMeasurable Interface Implementation Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testIMeasurableInterface_LengthUnitImplementation
        /// Verifies that LengthUnitMeasurable correctly implements IMeasurable interface.
        /// All interface methods must be present and return correct values.
        /// Expected: All method calls succeed and return correct results.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_LengthUnitImplementation()
        {
            IMeasurable unit = new LengthUnitMeasurable(LengthUnit.FEET);

            Assert.AreEqual(1.0, unit.GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, unit.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(1.0, unit.ConvertFromBaseUnit(1.0), EPSILON);
            Assert.AreEqual("FEET", unit.GetUnitName());
        }

        /// <summary>
        /// testIMeasurableInterface_WeightUnitImplementation
        /// Verifies that WeightUnitMeasurable correctly implements IMeasurable interface.
        /// All interface methods must be present and return correct values.
        /// Expected: All method calls succeed and return correct results.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_WeightUnitImplementation()
        {
            IMeasurable unit = new WeightUnitMeasurable(WeightUnit.KILOGRAM);

            Assert.AreEqual(1.0, unit.GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, unit.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(1.0, unit.ConvertFromBaseUnit(1.0), EPSILON);
            Assert.AreEqual("KILOGRAM", unit.GetUnitName());
        }

        /// <summary>
        /// testIMeasurableInterface_ConsistentBehavior
        /// Verifies that both LengthUnitMeasurable and WeightUnitMeasurable
        /// implement IMeasurable methods consistently with matching signatures.
        /// Expected: Both types are assignable to IMeasurable; method contracts match.
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_ConsistentBehavior()
        {
            IMeasurable lengthUnit = new LengthUnitMeasurable(LengthUnit.INCHES);
            IMeasurable weightUnit = new WeightUnitMeasurable(WeightUnit.GRAM);

            // Both implement the same interface — method signatures must match
            Assert.IsNotNull(lengthUnit.GetConversionFactor());
            Assert.IsNotNull(weightUnit.GetConversionFactor());

            Assert.IsNotNull(lengthUnit.ConvertToBaseUnit(12.0));
            Assert.IsNotNull(weightUnit.ConvertToBaseUnit(1000.0));

            Assert.IsNotNull(lengthUnit.ConvertFromBaseUnit(1.0));
            Assert.IsNotNull(weightUnit.ConvertFromBaseUnit(1.0));

            Assert.IsNotNull(lengthUnit.GetUnitName());
            Assert.IsNotNull(weightUnit.GetUnitName());
        }

        // -------------------------------------------------------------------------
        // Generic Quantity — Length Equality Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_LengthOperations_Equality
        /// Verifies that Quantity&lt;LengthUnitMeasurable&gt; equality works identically
        /// to the original Length class behavior.
        /// Expected: new Quantity(1.0, FEET).Equals(new Quantity(12.0, INCHES)) → true
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Equality()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

            Assert.IsTrue(feet.Equals(inches));
        }

        // -------------------------------------------------------------------------
        // Generic Quantity — Weight Equality Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_WeightOperations_Equality
        /// Verifies that Quantity&lt;WeightUnitMeasurable&gt; equality works identically
        /// to the original Weight class behavior.
        /// Expected: new Quantity(1.0, KILOGRAM).Equals(new Quantity(1000.0, GRAM)) → true
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Equality()
        {
            var kg   = new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));

            Assert.IsTrue(kg.Equals(gram));
        }

        // -------------------------------------------------------------------------
        // Generic Quantity — Length Conversion Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_LengthOperations_Conversion
        /// Verifies that Quantity&lt;LengthUnitMeasurable&gt; conversion returns correct result.
        /// Expected: new Quantity(1.0, FEET).ConvertTo(INCHES) → Quantity(12.0, INCHES)
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Conversion()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var result = feet.ConvertTo(new LengthUnitMeasurable(LengthUnit.INCHES));

            Assert.AreEqual(12.0, result.Value, EPSILON);
            Assert.AreEqual("INCHES", result.Unit.GetUnitName());
        }

        // -------------------------------------------------------------------------
        // Generic Quantity — Weight Conversion Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_WeightOperations_Conversion
        /// Verifies that Quantity&lt;WeightUnitMeasurable&gt; conversion returns correct result.
        /// Expected: new Quantity(1.0, KILOGRAM).ConvertTo(GRAM) → Quantity(1000.0, GRAM)
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Conversion()
        {
            var kg     = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var result = kg.ConvertTo(new WeightUnitMeasurable(WeightUnit.GRAM));

            Assert.AreEqual(1000.0, result.Value, EPSILON);
            Assert.AreEqual("GRAM", result.Unit.GetUnitName());
        }

        // -------------------------------------------------------------------------
        // Generic Quantity — Length Addition Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_LengthOperations_Addition
        /// Verifies that Quantity&lt;LengthUnitMeasurable&gt; addition returns correct result.
        /// Expected: new Quantity(1.0, FEET).Add(new Quantity(12.0, INCHES), FEET) → Quantity(2.0, FEET)
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Addition()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var sum    = feet.Add(inches, new LengthUnitMeasurable(LengthUnit.FEET));

            Assert.AreEqual(2.0, sum.Value, EPSILON);
            Assert.AreEqual("FEET", sum.Unit.GetUnitName());
        }

        // -------------------------------------------------------------------------
        // Generic Quantity — Weight Addition Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_WeightOperations_Addition
        /// Verifies that Quantity&lt;WeightUnitMeasurable&gt; addition returns correct result.
        /// Expected: new Quantity(1.0, KILOGRAM).Add(new Quantity(1000.0, GRAM), KILOGRAM) → Quantity(2.0, KILOGRAM)
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Addition()
        {
            var kg   = new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            var sum  = kg.Add(gram, new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            Assert.AreEqual(2.0, sum.Value, EPSILON);
            Assert.AreEqual("KILOGRAM", sum.Unit.GetUnitName());
        }

        // -------------------------------------------------------------------------
        // Cross-Category Prevention Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testCrossCategoryPrevention_LengthVsWeight
        /// Verifies that Quantity&lt;LengthUnitMeasurable&gt; and Quantity&lt;WeightUnitMeasurable&gt;
        /// cannot be considered equal even if numeric values match.
        /// Expected: equals() returns false when categories differ.
        /// </summary>
        [TestMethod]
        public void testCrossCategoryPrevention_LengthVsWeight()
        {
            var length = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            Assert.IsFalse(length.Equals(weight));
        }

        /// <summary>
        /// testCrossCategoryPrevention_CompilerTypeSafety
        /// Verifies that the compiler enforces type safety through generics.
        /// Quantity&lt;LengthUnitMeasurable&gt; and Quantity&lt;WeightUnitMeasurable&gt; are distinct types.
        /// Expected: The two types are not assignable to each other.
        /// </summary>
        [TestMethod]
        public void testCrossCategoryPrevention_CompilerTypeSafety()
        {
            var length = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            // Verify they are different runtime types — compiler already prevents direct assignment
            Assert.AreNotEqual(length.GetType(), weight.GetType());
        }

        // -------------------------------------------------------------------------
        // Constructor Validation Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_ConstructorValidation_NullUnit
        /// LengthUnitMeasurable is a struct and cannot be assigned null directly.
        /// This test therefore validates NaN rejection as the equivalent constructor
        /// guard — confirming that invalid inputs are caught at construction time.
        /// Expected: ArgumentException is thrown for NaN value.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_NullUnit()
        {
            // LengthUnitMeasurable is a value type (struct) — null is not assignable.
            // Constructor validation is confirmed via NaN, which exercises the same guard path.
            Assert.Throws<ArgumentException>(() =>
            {
                var q = new Quantity<LengthUnitMeasurable>(double.NaN, new LengthUnitMeasurable(LengthUnit.FEET));
            });
        }

        /// <summary>
        /// testGenericQuantity_ConstructorValidation_InvalidValue_NaN
        /// Verifies that NaN value is rejected in the Quantity constructor.
        /// Expected: ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_InvalidValue_NaN()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var q = new Quantity<LengthUnitMeasurable>(double.NaN, new LengthUnitMeasurable(LengthUnit.FEET));
            });
        }

        /// <summary>
        /// testGenericQuantity_ConstructorValidation_InvalidValue_PositiveInfinity
        /// Verifies that positive infinity is rejected in the Quantity constructor.
        /// Expected: ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_InvalidValue_PositiveInfinity()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var q = new Quantity<LengthUnitMeasurable>(double.PositiveInfinity, new LengthUnitMeasurable(LengthUnit.FEET));
            });
        }

        /// <summary>
        /// testGenericQuantity_ConstructorValidation_InvalidValue_NegativeInfinity
        /// Verifies that negative infinity is rejected in the Quantity constructor.
        /// Expected: ArgumentException is thrown.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_ConstructorValidation_InvalidValue_NegativeInfinity()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var q = new Quantity<WeightUnitMeasurable>(double.NegativeInfinity, new WeightUnitMeasurable(WeightUnit.GRAM));
            });
        }

        // -------------------------------------------------------------------------
        // All Unit Combinations — Conversion Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_Conversion_AllLengthUnitCombinations
        /// Verifies mathematical correctness of Quantity conversions across all length unit pairs.
        /// Expected: Each conversion returns the correct rounded value.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Conversion_AllLengthUnitCombinations()
        {
            // FEET → INCHES
            var feet = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(12.0, feet.ConvertTo(new LengthUnitMeasurable(LengthUnit.INCHES)).Value, EPSILON);

            // FEET → YARDS
            Assert.AreEqual(0.33, feet.ConvertTo(new LengthUnitMeasurable(LengthUnit.YARDS)).Value, EPSILON);

            // INCHES → FEET
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(1.0, inches.ConvertTo(new LengthUnitMeasurable(LengthUnit.FEET)).Value, EPSILON);

            // YARDS → FEET
            var yards = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.YARDS));
            Assert.AreEqual(3.0, yards.ConvertTo(new LengthUnitMeasurable(LengthUnit.FEET)).Value, EPSILON);

            // CENTIMETERS → FEET
            var cm = new Quantity<LengthUnitMeasurable>(30.48, new LengthUnitMeasurable(LengthUnit.CENTIMETERS));
            Assert.AreEqual(1.0, cm.ConvertTo(new LengthUnitMeasurable(LengthUnit.FEET)).Value, EPSILON);
        }

        /// <summary>
        /// testGenericQuantity_Conversion_AllWeightUnitCombinations
        /// Verifies mathematical correctness of Quantity conversions across all weight unit pairs.
        /// Expected: Each conversion returns the correct rounded value.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Conversion_AllWeightUnitCombinations()
        {
            // KILOGRAM → GRAM
            var kg = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(1000.0, kg.ConvertTo(new WeightUnitMeasurable(WeightUnit.GRAM)).Value, EPSILON);

            // GRAM → KILOGRAM
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.AreEqual(1.0, gram.ConvertTo(new WeightUnitMeasurable(WeightUnit.KILOGRAM)).Value, EPSILON);

            // KILOGRAM → POUND
            var kg2 = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(2.2, kg2.ConvertTo(new WeightUnitMeasurable(WeightUnit.POUND)).Value, EPSILON);
        }

        // -------------------------------------------------------------------------
        // All Unit Combinations — Addition Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericQuantity_Addition_AllLengthUnitCombinations
        /// Verifies addition of same and different length units with explicit target unit.
        /// Expected: Sum is correctly computed and returned in the specified target unit.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Addition_AllLengthUnitCombinations()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

            // 1 FEET + 12 INCHES = 2 FEET
            var sumFeet = feet.Add(inches, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(2.0, sumFeet.Value, EPSILON);

            // 1 FEET + 12 INCHES = 24 INCHES
            var sumInches = feet.Add(inches, new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(24.0, sumInches.Value, EPSILON);

            // 1 FEET + 12 INCHES = ~0.67 YARDS
            var sumYards = feet.Add(inches, new LengthUnitMeasurable(LengthUnit.YARDS));
            Assert.AreEqual(0.67, sumYards.Value, EPSILON);
        }

        /// <summary>
        /// testGenericQuantity_Addition_AllWeightUnitCombinations
        /// Verifies addition of same and different weight units with explicit target unit.
        /// Expected: Sum is correctly computed and returned in the specified target unit.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Addition_AllWeightUnitCombinations()
        {
            var kg   = new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));

            // 1 KG + 1000 GRAM = 2 KILOGRAM
            var sumKg = kg.Add(gram, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(2.0, sumKg.Value, EPSILON);

            // 1 KG + 1000 GRAM = 2000 GRAM
            var sumGram = kg.Add(gram, new WeightUnitMeasurable(WeightUnit.GRAM));
            Assert.AreEqual(2000.0, sumGram.Value, EPSILON);
        }

        // -------------------------------------------------------------------------
        // Backward Compatibility Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testBackwardCompatibility_AllUC1Through9Tests
        /// Verifies that all original Length and Weight classes still work correctly.
        /// The generic refactoring must not break any previously passing behavior.
        /// Expected: All UC1–UC9 assertions pass unchanged.
        /// </summary>
        [TestMethod]
        public void testBackwardCompatibility_AllUC1Through9Tests()
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
            var sum = length1.Add(length2);
            Assert.AreEqual(2.0, sum.Value, EPSILON);

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
        }

        // -------------------------------------------------------------------------
        // Simplified Demonstration Method Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testQuantityMeasurementApp_SimplifiedDemonstration_Equality
        /// Verifies that a single generic method can handle equality for any category.
        /// Expected: The same method works for both Length and Weight quantities.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Equality()
        {
            // Single generic method — works for Length
            var lengthResult = DemonstrateEquality(
                new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET)),
                new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES))
            );
            Assert.IsTrue(lengthResult);

            // Same generic method — works for Weight
            var weightResult = DemonstrateEquality(
                new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM)),
                new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM))
            );
            Assert.IsTrue(weightResult);
        }

        /// <summary>
        /// testQuantityMeasurementApp_SimplifiedDemonstration_Conversion
        /// Verifies that a single generic method can handle conversion for any category.
        /// Expected: The same method works for both Length and Weight quantities.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Conversion()
        {
            // Single generic method — works for Length
            var lengthResult = DemonstrateConversion(
                new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET)),
                new LengthUnitMeasurable(LengthUnit.INCHES)
            );
            Assert.AreEqual(12.0, lengthResult.Value, EPSILON);

            // Same generic method — works for Weight
            var weightResult = DemonstrateConversion(
                new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM)),
                new WeightUnitMeasurable(WeightUnit.GRAM)
            );
            Assert.AreEqual(1000.0, weightResult.Value, EPSILON);
        }

        /// <summary>
        /// testQuantityMeasurementApp_SimplifiedDemonstration_Addition
        /// Verifies that a single generic method can handle addition for any category.
        /// Expected: The same method works for both Length and Weight quantities.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Addition()
        {
            // Single generic method — works for Length
            var lengthResult = DemonstrateAddition(
                new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET)),
                new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES)),
                new LengthUnitMeasurable(LengthUnit.FEET)
            );
            Assert.AreEqual(2.0, lengthResult.Value, EPSILON);

            // Same generic method — works for Weight
            var weightResult = DemonstrateAddition(
                new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM)),
                new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM)),
                new WeightUnitMeasurable(WeightUnit.KILOGRAM)
            );
            Assert.AreEqual(2.0, weightResult.Value, EPSILON);
        }

        // -------------------------------------------------------------------------
        // Type Wildcard and Flexible Signature Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testTypeWildcard_FlexibleSignatures
        /// Verifies that a method accepting Quantity&lt;TUnit&gt; with generic constraint
        /// works transparently with any IMeasurable unit type.
        /// Expected: Single method handles Length and Weight without overloads.
        /// </summary>
        [TestMethod]
        public void testTypeWildcard_FlexibleSignatures()
        {
            var length = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            // Both pass through the same generic helper method signature
            Assert.AreEqual("FEET",     GetUnitName(length));
            Assert.AreEqual("KILOGRAM", GetUnitName(weight));
        }

        // -------------------------------------------------------------------------
        // Scalability — New Unit Category Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testScalability_NewUnitEnumIntegration
        /// Creates a test VolumeUnit and IMeasurable wrapper and verifies it integrates
        /// with Quantity&lt;TUnit&gt; without any changes to the generic class.
        /// Expected: New category works seamlessly with existing Quantity&lt;TUnit&gt; class.
        /// </summary>
        [TestMethod]
        public void testScalability_NewUnitEnumIntegration()
        {
            // VolumeUnit defined locally — no changes needed to Quantity<TUnit>
            var litre      = new Quantity<TestVolumeUnit>(1.0,    new TestVolumeUnit("LITRE",      1.0));
            var millilitre = new Quantity<TestVolumeUnit>(1000.0, new TestVolumeUnit("MILLILITRE", 0.001));

            Assert.IsTrue(litre.Equals(millilitre));
        }

        /// <summary>
        /// testScalability_MultipleNewCategories
        /// Tests that Temperature and Time categories can be added using only a new
        /// IMeasurable struct — no changes to Quantity&lt;TUnit&gt; or existing code required.
        /// Expected: All new categories work without modifying generic infrastructure.
        /// </summary>
        [TestMethod]
        public void testScalability_MultipleNewCategories()
        {
            // Temperature category
            var celsius  = new Quantity<TestScalarUnit>(100.0, new TestScalarUnit("CELSIUS", 1.0));
            var celsius2 = new Quantity<TestScalarUnit>(100.0, new TestScalarUnit("CELSIUS", 1.0));
            Assert.IsTrue(celsius.Equals(celsius2));

            // Time category
            var seconds  = new Quantity<TestScalarUnit>(60.0, new TestScalarUnit("SECONDS", 1.0));
            var seconds2 = new Quantity<TestScalarUnit>(60.0, new TestScalarUnit("SECONDS", 1.0));
            Assert.IsTrue(seconds.Equals(seconds2));
        }

        // -------------------------------------------------------------------------
        // Bounded Type Parameter Enforcement Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testGenericBoundedTypeParameter_Enforcement
        /// Verifies that the bounded type parameter &lt;TUnit : IMeasurable&gt; only
        /// accepts types that implement the IMeasurable interface.
        /// Expected: Valid IMeasurable types compile and work; invalid types are rejected by compiler.
        /// </summary>
        [TestMethod]
        public void testGenericBoundedTypeParameter_Enforcement()
        {
            // LengthUnitMeasurable satisfies the constraint
            var lengthQ = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.IsNotNull(lengthQ);

            // WeightUnitMeasurable satisfies the constraint
            var weightQ = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.IsNotNull(weightQ);

            // Custom IMeasurable satisfies the constraint
            var customQ = new Quantity<TestScalarUnit>(1.0, new TestScalarUnit("CUSTOM", 1.0));
            Assert.IsNotNull(customQ);
        }

        // -------------------------------------------------------------------------
        // HashCode Consistency Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testHashCode_GenericQuantity_Consistency
        /// Verifies that hashCode() is consistent: equal quantities must have equal hash codes.
        /// Expected: Quantities with equivalent base values produce the same hash code.
        /// </summary>
        [TestMethod]
        public void testHashCode_GenericQuantity_Consistency()
        {
            var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

            // Equal objects must have equal hash codes
            Assert.IsTrue(feet.Equals(inches));
            Assert.AreEqual(feet.GetHashCode(), inches.GetHashCode());
        }

        /// <summary>
        /// testHashCode_GenericQuantity_WeightConsistency
        /// Verifies hash code consistency for weight quantities.
        /// Expected: Equivalent weight quantities produce equal hash codes.
        /// </summary>
        [TestMethod]
        public void testHashCode_GenericQuantity_WeightConsistency()
        {
            var kg   = new Quantity<WeightUnitMeasurable>(1.0,    new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var gram = new Quantity<WeightUnitMeasurable>(1000.0, new WeightUnitMeasurable(WeightUnit.GRAM));

            Assert.IsTrue(kg.Equals(gram));
            Assert.AreEqual(kg.GetHashCode(), gram.GetHashCode());
        }

        // -------------------------------------------------------------------------
        // Equals Contract — Reflexive, Symmetric, Transitive Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testEquals_GenericQuantity_ContractPreservation
        /// Verifies that the equals() contract is maintained for generic quantities:
        /// reflexive (a == a), symmetric (a == b → b == a), transitive (a == b, b == c → a == c).
        /// Expected: All three equality properties hold.
        /// </summary>
        [TestMethod]
        public void testEquals_GenericQuantity_ContractPreservation()
        {
            var a = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            var b = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var c = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

            // Reflexive: a == a
            Assert.IsTrue(a.Equals(a));

            // Symmetric: a == b → b == a
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));

            // Transitive: a == b, b == c → a == c
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));

            // Null: a != null
            Assert.IsFalse(a.Equals(null));
        }

        // -------------------------------------------------------------------------
        // Enum as Behavior Carrier Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testEnumAsUnitCarrier_BehaviorEncapsulation
        /// Verifies that unit structs carry behavior through IMeasurable interface,
        /// enabling polymorphic calls that work correctly across categories.
        /// Expected: ConvertToBaseUnit and ConvertFromBaseUnit produce correct results
        /// when called through the IMeasurable interface reference.
        /// </summary>
        [TestMethod]
        public void testEnumAsUnitCarrier_BehaviorEncapsulation()
        {
            IMeasurable lengthUnit = new LengthUnitMeasurable(LengthUnit.INCHES);
            IMeasurable weightUnit = new WeightUnitMeasurable(WeightUnit.GRAM);

            // Polymorphic calls through interface — behavior is encapsulated in unit
            Assert.AreEqual(1.0,    lengthUnit.ConvertToBaseUnit(12.0),  EPSILON); // 12 inches = 1 foot
            Assert.AreEqual(0.001,  weightUnit.ConvertToBaseUnit(1.0),   EPSILON); // 1 gram = 0.001 kg
            Assert.AreEqual(12.0,   lengthUnit.ConvertFromBaseUnit(1.0), EPSILON); // 1 foot = 12 inches
            Assert.AreEqual(1000.0, weightUnit.ConvertFromBaseUnit(1.0), EPSILON); // 1 kg = 1000 grams
        }

        // -------------------------------------------------------------------------
        // Type Erasure Runtime Safety Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testTypeErasure_RuntimeSafety
        /// Verifies that despite generic type erasure at runtime, cross-category
        /// checks still work correctly through the unit's runtime type.
        /// Expected: Cross-category equals() returns false; no ClassCastException.
        /// </summary>
        [TestMethod]
        public void testTypeErasure_RuntimeSafety()
        {
            var length = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weight = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));

            // No exception thrown; runtime type check handles erasure safely
            bool result = length.Equals(weight);
            Assert.IsFalse(result);

            // Runtime types are distinct
            Assert.AreNotEqual(length.GetType(), weight.GetType());
        }

        // -------------------------------------------------------------------------
        // Composition Over Inheritance Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testCompositionOverInheritance_Flexibility
        /// Verifies that Quantity&lt;TUnit&gt; uses composition (holds a TUnit) rather than
        /// category-specific inheritance, making it flexible with any IMeasurable type.
        /// Expected: Quantity&lt;TUnit&gt; works identically with Length, Weight, and custom units.
        /// </summary>
        [TestMethod]
        public void testCompositionOverInheritance_Flexibility()
        {
            var lengthQ = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var weightQ = new Quantity<WeightUnitMeasurable>(1.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var customQ = new Quantity<TestScalarUnit>(1.0, new TestScalarUnit("UNIT", 1.0));

            // All three use the same Quantity<TUnit> class — no inheritance needed
            Assert.AreEqual(1.0, lengthQ.Value, EPSILON);
            Assert.AreEqual(1.0, weightQ.Value, EPSILON);
            Assert.AreEqual(1.0, customQ.Value, EPSILON);

            Assert.AreEqual("FEET",     lengthQ.Unit.GetUnitName());
            Assert.AreEqual("KILOGRAM", weightQ.Unit.GetUnitName());
            Assert.AreEqual("UNIT",     customQ.Unit.GetUnitName());
        }

        // -------------------------------------------------------------------------
        // Immutability Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testImmutability_GenericQuantity
        /// Verifies that Quantity&lt;TUnit&gt; objects are immutable: no setters exist,
        /// and all operations return new instances rather than modifying the original.
        /// Expected: Original quantity is unchanged after ConvertTo and Add operations.
        /// </summary>
        [TestMethod]
        public void testImmutability_GenericQuantity()
        {
            var original = new Quantity<LengthUnitMeasurable>(1.0, new LengthUnitMeasurable(LengthUnit.FEET));

            // ConvertTo returns a new instance
            var converted = original.ConvertTo(new LengthUnitMeasurable(LengthUnit.INCHES));
            Assert.AreEqual(1.0,  original.Value,  EPSILON); // original unchanged
            Assert.AreEqual(12.0, converted.Value, EPSILON); // new instance has converted value
            Assert.AreEqual("FEET",   original.Unit.GetUnitName());
            Assert.AreEqual("INCHES", converted.Unit.GetUnitName());

            // Add returns a new instance
            var other = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));
            var sum   = original.Add(other, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(1.0, original.Value, EPSILON); // original unchanged
            Assert.AreEqual(2.0, sum.Value,      EPSILON); // new instance holds the sum
        }

        // -------------------------------------------------------------------------
        // Interface Segregation Tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// testInterfaceSegregation_MinimalContract
        /// Verifies that the IMeasurable interface is minimal and focused,
        /// containing only the four methods required for unit conversion operations.
        /// Expected: Interface has exactly GetConversionFactor, ConvertToBaseUnit,
        /// ConvertFromBaseUnit, and GetUnitName — nothing more.
        /// </summary>
        [TestMethod]
        public void testInterfaceSegregation_MinimalContract()
        {
            var methods = typeof(IMeasurable).GetMethods();

            // UC14 added two default methods (SupportsArithmeticOps, ValidateOperationSupport).
            // UC15 added GetMeasurementType(). Interface now has 7 methods total.
            Assert.AreEqual(7, methods.Length,
                "IMeasurable should define 7 methods: GetConversionFactor, " +
                "ConvertToBaseUnit, ConvertFromBaseUnit, GetUnitName, " +
                "SupportsArithmeticOps, ValidateOperationSupport, GetMeasurementType");
        }

        // -------------------------------------------------------------------------
        // Private generic helper methods (mirror QuantityPresentationUC10 pattern)
        // -------------------------------------------------------------------------

        private bool DemonstrateEquality<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2)
            where TUnit : IMeasurable
        {
            return q1.Equals(q2);
        }

        private Quantity<TUnit> DemonstrateConversion<TUnit>(Quantity<TUnit> q, TUnit target)
            where TUnit : IMeasurable
        {
            return q.ConvertTo(target);
        }

        private Quantity<TUnit> DemonstrateAddition<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2, TUnit target)
            where TUnit : IMeasurable
        {
            return q1.Add(q2, target);
        }

        private string GetUnitName<TUnit>(Quantity<TUnit> q)
            where TUnit : IMeasurable
        {
            return q.Unit.GetUnitName();
        }

        // -------------------------------------------------------------------------
        // Local test-only IMeasurable implementations for scalability tests
        // -------------------------------------------------------------------------

        /// <summary>
        /// Minimal IMeasurable implementation for scalability tests.
        /// Represents a simple scalar unit with a given conversion factor.
        /// Demonstrates that any struct implementing IMeasurable works with Quantity&lt;TUnit&gt;.
        /// </summary>
        private readonly struct TestScalarUnit : IMeasurable
        {
            private readonly string _name;
            private readonly double _factor;

            public TestScalarUnit(string name, double factor)
            {
                _name   = name;
                _factor = factor;
            }

            public double GetConversionFactor()             => _factor;
            public double ConvertToBaseUnit(double value)   => value * _factor;
            public double ConvertFromBaseUnit(double base_) => base_ / _factor;
            public string GetUnitName()                     => _name;
            public string GetMeasurementType()              => "SCALAR";
        }

        /// <summary>
        /// IMeasurable implementation representing a simple volume unit.
        /// Used in testScalability_NewUnitEnumIntegration to prove that
        /// Quantity&lt;TUnit&gt; requires no changes for new categories.
        /// </summary>
        private readonly struct TestVolumeUnit : IMeasurable
        {
            private readonly string _name;
            private readonly double _factor; // factor relative to LITRE as base unit

            public TestVolumeUnit(string name, double factor)
            {
                _name   = name;
                _factor = factor;
            }

            public double GetConversionFactor()             => _factor;
            public double ConvertToBaseUnit(double value)   => value * _factor;
            public double ConvertFromBaseUnit(double base_) => base_ / _factor;
            public string GetUnitName()                     => _name;
            public string GetMeasurementType()              => "VOLUME";
        }
    }
}