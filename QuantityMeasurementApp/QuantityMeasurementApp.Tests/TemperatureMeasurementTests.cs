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
    /// UC14: Exactly 41 test cases covering Temperature measurement.
    /// Framework: MSTest
    /// </summary>
    [TestClass]
    public class TemperatureMeasurementTests
    {
        private const double EPSILON = 1e-4;

        // ── Helpers ───────────────────────────────────────────────────────────
        private static Quantity<TemperatureUnitMeasurable> C(double v) =>
            new Quantity<TemperatureUnitMeasurable>(v, new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS));

        private static Quantity<TemperatureUnitMeasurable> F(double v) =>
            new Quantity<TemperatureUnitMeasurable>(v, new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT));

        private static Quantity<TemperatureUnitMeasurable> K(double v) =>
            new Quantity<TemperatureUnitMeasurable>(v, new TemperatureUnitMeasurable(TemperatureUnit.KELVIN));

        private static TemperatureUnitMeasurable CU => new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS);
        private static TemperatureUnitMeasurable FU => new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT);
        private static TemperatureUnitMeasurable KU => new TemperatureUnitMeasurable(TemperatureUnit.KELVIN);

        // ─────────────────────────────────────────────────────────────────────
        // 1. testTemperatureEquality_CelsiusToCelsius_SameValue
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Verifies 0°C equals 0°C.</summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            Assert.IsTrue(C(0.0).Equals(C(0.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 2. testTemperatureEquality_FahrenheitToFahrenheit_SameValue
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Verifies 32°F equals 32°F.</summary>
        [TestMethod]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            Assert.IsTrue(F(32.0).Equals(F(32.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 3. testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Verifies 0°C equals 32°F (freezing point).</summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            Assert.IsTrue(C(0.0).Equals(F(32.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 4. testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Verifies 100°C equals 212°F (boiling point).</summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            Assert.IsTrue(C(100.0).Equals(F(212.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 5. testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Verifies -40°C equals -40°F (the only intersection of the two scales).</summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            Assert.IsTrue(C(-40.0).Equals(F(-40.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 6. testTemperatureEquality_SymmetricProperty
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>If A equals B then B equals A.</summary>
        [TestMethod]
        public void testTemperatureEquality_SymmetricProperty()
        {
            Assert.IsTrue(C(0.0).Equals(F(32.0)));
            Assert.IsTrue(F(32.0).Equals(C(0.0)));

            Assert.IsTrue(C(100.0).Equals(F(212.0)));
            Assert.IsTrue(F(212.0).Equals(C(100.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 7. testTemperatureEquality_ReflexiveProperty
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>A temperature equals itself.</summary>
        [TestMethod]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            var t = C(100.0);
            Assert.IsTrue(t.Equals(t));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 8. testTemperatureConversion_CelsiusToFahrenheit_VariousValues
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Tests multiple C→F conversions: 50→122, -20→-4, 37→98.6, 0→32.</summary>
        [TestMethod]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            Assert.AreEqual(122.0,  C(50.0).ConvertTo(FU).Value,  EPSILON);
            Assert.AreEqual(-4.0,   C(-20.0).ConvertTo(FU).Value, EPSILON);
            Assert.AreEqual(98.6,   C(37.0).ConvertTo(FU).Value,  EPSILON);
            Assert.AreEqual(32.0,   C(0.0).ConvertTo(FU).Value,   EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 9. testTemperatureConversion_FahrenheitToCelsius_VariousValues
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Tests multiple F→C conversions: 122→50, -4→-20, 98.6→37, 32→0.</summary>
        [TestMethod]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            Assert.AreEqual(50.0,   F(122.0).ConvertTo(CU).Value,  EPSILON);
            Assert.AreEqual(-20.0,  F(-4.0).ConvertTo(CU).Value,   EPSILON);
            Assert.AreEqual(37.0,   F(98.6).ConvertTo(CU).Value,   EPSILON);
            Assert.AreEqual(0.0,    F(32.0).ConvertTo(CU).Value,   EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 10. testTemperatureConversion_RoundTrip_PreservesValue
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>convert(convert(value, C, F), F, C) ≈ original value.</summary>
        [TestMethod]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            double original   = 37.0;
            double roundTrip  = C(original).ConvertTo(FU).ConvertTo(CU).Value;
            Assert.AreEqual(original, roundTrip, EPSILON);

            double original2  = -40.0;
            double roundTrip2 = C(original2).ConvertTo(FU).ConvertTo(CU).Value;
            Assert.AreEqual(original2, roundTrip2, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 11. testTemperatureConversion_SameUnit
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Converting to the same unit returns the unchanged value.</summary>
        [TestMethod]
        public void testTemperatureConversion_SameUnit()
        {
            Assert.AreEqual(25.0,   C(25.0).ConvertTo(CU).Value,   EPSILON);
            Assert.AreEqual(77.0,   F(77.0).ConvertTo(FU).Value,   EPSILON);
            Assert.AreEqual(300.0,  K(300.0).ConvertTo(KU).Value,  EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 12. testTemperatureConversion_ZeroValue
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>0°C → 32°F and 0°F → -17.78°C.</summary>
        [TestMethod]
        public void testTemperatureConversion_ZeroValue()
        {
            Assert.AreEqual(32.0,   C(0.0).ConvertTo(FU).Value,  EPSILON);
            Assert.AreEqual(-17.78, F(0.0).ConvertTo(CU).Value,  EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 13. testTemperatureConversion_NegativeValues
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Tests negative temperature conversions.</summary>
        [TestMethod]
        public void testTemperatureConversion_NegativeValues()
        {
            Assert.AreEqual(-40.0,   C(-40.0).ConvertTo(FU).Value, EPSILON);
            Assert.AreEqual(-4.0,    C(-20.0).ConvertTo(FU).Value, EPSILON);
            Assert.AreEqual(-273.15, K(0.0).ConvertTo(CU).Value,   EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 14. testTemperatureConversion_LargeValues
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Tests very high temperatures (e.g. 1000°C → 1832°F).</summary>
        [TestMethod]
        public void testTemperatureConversion_LargeValues()
        {
            Assert.AreEqual(1832.0,  C(1000.0).ConvertTo(FU).Value, EPSILON);
            Assert.AreEqual(1273.15, C(1000.0).ConvertTo(KU).Value, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 15. testTemperatureUnsupportedOperation_Add
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>add() throws NotSupportedException. Tests error message clarity.</summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_Add()
        {
            var ex = Assert.Throws<NotSupportedException>(() => C(100.0).Add(C(50.0)));
            Assert.IsTrue(ex.Message.Contains("Temperature"),  "Message must mention Temperature");
            Assert.IsTrue(ex.Message.Contains("Add"),          "Message must mention Add");
        }

        // ─────────────────────────────────────────────────────────────────────
        // 16. testTemperatureUnsupportedOperation_Subtract
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>subtract() throws NotSupportedException.</summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_Subtract()
        {
            Assert.Throws<NotSupportedException>(() => C(100.0).Subtract(C(50.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 17. testTemperatureUnsupportedOperation_Divide
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>divide() throws NotSupportedException.</summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_Divide()
        {
            Assert.Throws<NotSupportedException>(() => C(100.0).Divide(C(50.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 18. testTemperatureUnsupportedOperation_ErrorMessage
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Error message explains why the operation is unsupported.</summary>
        [TestMethod]
        public void testTemperatureUnsupportedOperation_ErrorMessage()
        {
            var exAdd = Assert.Throws<NotSupportedException>(() => C(100.0).Add(C(50.0)));
            Assert.IsTrue(exAdd.Message.Contains("Temperature"), "Message must mention Temperature");
            Assert.IsTrue(exAdd.Message.Contains("Add"),         "Message must name the operation");

            var exSub = Assert.Throws<NotSupportedException>(() => C(100.0).Subtract(C(50.0)));
            Assert.IsTrue(exSub.Message.Contains("Subtract"));

            var exDiv = Assert.Throws<NotSupportedException>(() => C(100.0).Divide(C(50.0)));
            Assert.IsTrue(exDiv.Message.Contains("Divide"));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 19. testTemperatureVsLengthIncompatibility
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>100°C does not equal 100 FEET.</summary>
        [TestMethod]
        public void testTemperatureVsLengthIncompatibility()
        {
            var temp   = C(100.0);
            var length = new Quantity<LengthUnitMeasurable>(100.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.IsFalse(temp.Equals(length));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 20. testTemperatureVsWeightIncompatibility
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>50°C does not equal 50 KILOGRAM.</summary>
        [TestMethod]
        public void testTemperatureVsWeightIncompatibility()
        {
            var temp   = C(50.0);
            var weight = new Quantity<WeightUnitMeasurable>(50.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.IsFalse(temp.Equals(weight));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 21. testTemperatureVsVolumeIncompatibility
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>25°C does not equal 25 LITRE.</summary>
        [TestMethod]
        public void testTemperatureVsVolumeIncompatibility()
        {
            var temp   = C(25.0);
            var volume = new Quantity<VolumeUnitMeasurable>(25.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.IsFalse(temp.Equals(volume));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 22. testOperationSupportMethods_TemperatureUnitAddition
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>TemperatureUnit CELSIUS SupportsArithmeticOps() returns false.</summary>
        [TestMethod]
        public void testOperationSupportMethods_TemperatureUnitAddition()
        {
            var unit = new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS);
            Assert.IsFalse(unit.SupportsArithmeticOps());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 23. testOperationSupportMethods_TemperatureUnitDivision
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>TemperatureUnit FAHRENHEIT SupportsArithmeticOps() returns false.</summary>
        [TestMethod]
        public void testOperationSupportMethods_TemperatureUnitDivision()
        {
            var unit = new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT);
            Assert.IsFalse(unit.SupportsArithmeticOps());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 24. testOperationSupportMethods_LengthUnitAddition
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>LengthUnit FEET SupportsArithmeticOps() returns true (inherited default interface method).</summary>
        [TestMethod]
        public void testOperationSupportMethods_LengthUnitAddition()
        {
            // Cast to IMeasurable — C# requires calling default interface methods through the interface type
            IMeasurable unit = new LengthUnitMeasurable(LengthUnit.FEET);
            Assert.IsTrue(unit.SupportsArithmeticOps());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 25. testOperationSupportMethods_WeightUnitDivision
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>WeightUnit KILOGRAM SupportsArithmeticOps() returns true (inherited default interface method).</summary>
        [TestMethod]
        public void testOperationSupportMethods_WeightUnitDivision()
        {
            // Cast to IMeasurable — C# requires calling default interface methods through the interface type
            IMeasurable unit = new WeightUnitMeasurable(WeightUnit.KILOGRAM);
            Assert.IsTrue(unit.SupportsArithmeticOps());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 26. testIMeasurableInterface_Evolution_BackwardCompatible
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Existing units work without modification after UC14 IMeasurable refactor.</summary>
        [TestMethod]
        public void testIMeasurableInterface_Evolution_BackwardCompatible()
        {
            // Length arithmetic unchanged
            var la = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var lb = new Quantity<LengthUnitMeasurable>(5.0,  new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(15.0, la.Add(lb).Value,      EPSILON);
            Assert.AreEqual(5.0,  la.Subtract(lb).Value, EPSILON);
            Assert.AreEqual(2.0,  la.Divide(lb),          EPSILON);

            // Volume arithmetic unchanged
            var va = new Quantity<VolumeUnitMeasurable>(6.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var vb = new Quantity<VolumeUnitMeasurable>(2.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(8.0, va.Add(vb).Value,      EPSILON);
            Assert.AreEqual(4.0, va.Subtract(vb).Value, EPSILON);
            Assert.AreEqual(3.0, va.Divide(vb),          EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 27. testTemperatureUnit_NonLinearConversion
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Temperature conversions use formulas, not simple multiplication.
        /// If linear (×factor), 100°C × factor ≠ 212°F.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnit_NonLinearConversion()
        {
            // If conversion were purely multiplicative, 100°C × some factor would never give 212°F
            // because the Fahrenheit scale has an offset of 32.
            double result = C(100.0).ConvertTo(FU).Value;
            Assert.AreEqual(212.0, result, EPSILON);

            // Verify the offset matters: 0°C → 32°F (not 0°F)
            Assert.AreNotEqual(0.0, C(0.0).ConvertTo(FU).Value, EPSILON);
            Assert.AreEqual(32.0,   C(0.0).ConvertTo(FU).Value, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 28. testTemperatureUnit_AllConstants
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>CELSIUS and FAHRENHEIT (and KELVIN) enum constants are accessible.</summary>
        [TestMethod]
        public void testTemperatureUnit_AllConstants()
        {
            Assert.AreEqual(TemperatureUnit.CELSIUS,    new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS).Unit);
            Assert.AreEqual(TemperatureUnit.FAHRENHEIT, new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT).Unit);
            Assert.AreEqual(TemperatureUnit.KELVIN,     new TemperatureUnitMeasurable(TemperatureUnit.KELVIN).Unit);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 29. testTemperatureUnit_NameMethod
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>GetUnitName() returns the correct name for each constant.</summary>
        [TestMethod]
        public void testTemperatureUnit_NameMethod()
        {
            Assert.AreEqual("CELSIUS",    new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS).GetUnitName());
            Assert.AreEqual("FAHRENHEIT", new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT).GetUnitName());
            Assert.AreEqual("KELVIN",     new TemperatureUnitMeasurable(TemperatureUnit.KELVIN).GetUnitName());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 30. testTemperatureUnit_ConversionFactor
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>GetConversionFactor() returns 1.0 for all temperature units (nominal placeholder).</summary>
        [TestMethod]
        public void testTemperatureUnit_ConversionFactor()
        {
            Assert.AreEqual(1.0, new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS).GetConversionFactor(),    EPSILON);
            Assert.AreEqual(1.0, new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT).GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, new TemperatureUnitMeasurable(TemperatureUnit.KELVIN).GetConversionFactor(),     EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 31. testTemperatureNullUnitValidation
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// TemperatureUnitMeasurable is a struct so default() is not null — no ArgumentNullException.
        /// Instead verify that UNKNOWN unit throws ArgumentException when a conversion is attempted.
        /// </summary>
        [TestMethod]
        public void testTemperatureNullUnitValidation()
        {
            // default(TemperatureUnitMeasurable) has Unit = TemperatureUnit.UNKNOWN (value 0).
            // Constructing a Quantity with it succeeds (struct is not null).
            // Attempting to convert it throws ArgumentException because UNKNOWN is rejected.
            var q = new Quantity<TemperatureUnitMeasurable>(100.0, default(TemperatureUnitMeasurable));
            Assert.Throws<ArgumentException>(() => q.ToBaseUnit());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 32. testTemperatureNullOperandValidation_InComparison
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>equals(null) returns false.</summary>
        [TestMethod]
        public void testTemperatureNullOperandValidation_InComparison()
        {
            Assert.IsFalse(C(100.0).Equals(null));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 33. testTemperatureDifferentValuesInequality
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>50°C is not equal to 100°C.</summary>
        [TestMethod]
        public void testTemperatureDifferentValuesInequality()
        {
            Assert.IsFalse(C(50.0).Equals(C(100.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 34. testTemperatureBackwardCompatibility_UC1_Through_UC13
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Temperature additions do not break any existing categories.</summary>
        [TestMethod]
        public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
        {
            // Length (UC6/UC12)
            var la = new Quantity<LengthUnitMeasurable>(9.0, new LengthUnitMeasurable(LengthUnit.FEET));
            var lb = new Quantity<LengthUnitMeasurable>(3.0, new LengthUnitMeasurable(LengthUnit.FEET));
            Assert.AreEqual(12.0, la.Add(lb).Value,      EPSILON);
            Assert.AreEqual(6.0,  la.Subtract(lb).Value, EPSILON);
            Assert.AreEqual(3.0,  la.Divide(lb),          EPSILON);

            // Weight (UC9/UC12)
            var wa = new Quantity<WeightUnitMeasurable>(9.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            var wb = new Quantity<WeightUnitMeasurable>(3.0, new WeightUnitMeasurable(WeightUnit.KILOGRAM));
            Assert.AreEqual(12.0, wa.Add(wb).Value,      EPSILON);
            Assert.AreEqual(6.0,  wa.Subtract(wb).Value, EPSILON);
            Assert.AreEqual(3.0,  wa.Divide(wb),          EPSILON);

            // Volume (UC11/UC12)
            var va = new Quantity<VolumeUnitMeasurable>(9.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            var vb = new Quantity<VolumeUnitMeasurable>(3.0, new VolumeUnitMeasurable(VolumeUnit.LITRE));
            Assert.AreEqual(12.0, va.Add(vb).Value,      EPSILON);
            Assert.AreEqual(6.0,  va.Subtract(vb).Value, EPSILON);
            Assert.AreEqual(3.0,  va.Divide(vb),          EPSILON);

            // Temperature equality and conversion still work
            Assert.IsTrue(C(0.0).Equals(F(32.0)));
            Assert.AreEqual(212.0, C(100.0).ConvertTo(FU).Value, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 35. testTemperatureConversionPrecision_Epsilon
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Epsilon-based tolerance confirms equality after floating-point conversion.</summary>
        [TestMethod]
        public void testTemperatureConversionPrecision_Epsilon()
        {
            // 50°C → 122°F; equality check via base-unit comparison uses epsilon
            Assert.IsTrue(C(50.0).Equals(F(122.0)));

            // Round-trip introduces tiny floating-point error — epsilon absorbs it
            var roundTripped = C(37.0).ConvertTo(FU).ConvertTo(CU);
            Assert.AreEqual(37.0, roundTripped.Value, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 36. testTemperatureConversionEdgeCase_VerySmallDifference
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Two temperatures that differ by less than the internal epsilon (1e-9) are treated as equal.
        /// Two temperatures that differ by more are not equal.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversionEdgeCase_VerySmallDifference()
        {
            // Differ by 1e-10 — smaller than the internal epsilon (1e-9), so treated as equal
            Assert.IsTrue(C(100.0000000001).Equals(C(100.0)));

            // Differ by 0.5 — clearly outside epsilon, not equal
            Assert.IsFalse(C(100.5).Equals(C(100.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 37. testTemperatureEnumImplementsIMeasurable
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>TemperatureUnitMeasurable correctly implements IMeasurable.</summary>
        [TestMethod]
        public void testTemperatureEnumImplementsIMeasurable()
        {
            IMeasurable unit = new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS);
            Assert.IsNotNull(unit);
            Assert.IsNotNull(unit.GetUnitName());
            // ConvertToBaseUnit and ConvertFromBaseUnit are callable via the interface
            double baseVal      = unit.ConvertToBaseUnit(100.0);
            double restoredVal  = unit.ConvertFromBaseUnit(baseVal);
            Assert.AreEqual(100.0, restoredVal, EPSILON);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 38. testTemperatureDefaultMethodInheritance
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Non-temperature units inherit default true from IMeasurable.SupportsArithmeticOps().</summary>
        [TestMethod]
        public void testTemperatureDefaultMethodInheritance()
        {
            IMeasurable length = new LengthUnitMeasurable(LengthUnit.FEET);
            IMeasurable weight = new WeightUnitMeasurable(WeightUnit.KILOGRAM);
            IMeasurable volume = new VolumeUnitMeasurable(VolumeUnit.LITRE);

            Assert.IsTrue(length.SupportsArithmeticOps());
            Assert.IsTrue(weight.SupportsArithmeticOps());
            Assert.IsTrue(volume.SupportsArithmeticOps());

            // Temperature overrides to false
            IMeasurable temp = new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS);
            Assert.IsFalse(temp.SupportsArithmeticOps());
        }

        // ─────────────────────────────────────────────────────────────────────
        // 39. testTemperatureCrossUnitAdditionAttempt
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Adding KELVIN to CELSIUS throws NotSupportedException (different temperature units).</summary>
        [TestMethod]
        public void testTemperatureCrossUnitAdditionAttempt()
        {
            Assert.Throws<NotSupportedException>(() => K(23.0).Add(C(45.0)));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 40. testTemperatureValidateOperationSupport_MethodBehavior
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Direct test: TemperatureUnitMeasurable.ValidateOperationSupport("addition") throws NotSupportedException.
        /// </summary>
        [TestMethod]
        public void testTemperatureValidateOperationSupport_MethodBehavior()
        {
            var unit = new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS);
            var ex   = Assert.Throws<NotSupportedException>(() => unit.ValidateOperationSupport("addition"));
            Assert.IsTrue(ex.Message.Contains("Temperature"));
            Assert.IsTrue(ex.Message.Contains("addition"));
        }

        // ─────────────────────────────────────────────────────────────────────
        // 41. testTemperatureIntegrationWithGenericQuantity
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Quantity&lt;TemperatureUnitMeasurable&gt; works seamlessly with the generic class.</summary>
        [TestMethod]
        public void testTemperatureIntegrationWithGenericQuantity()
        {
            // Construction
            var q = C(100.0);
            Assert.AreEqual(100.0,     q.Value,            EPSILON);
            Assert.AreEqual("CELSIUS", q.Unit.GetUnitName());

            // ToString
            Assert.AreEqual("100.00 CELSIUS", q.ToString());

            // ToBaseUnit (CELSIUS is base, so returns value unchanged)
            Assert.AreEqual(100.0, q.ToBaseUnit(), EPSILON);

            // ConvertTo
            var inF = q.ConvertTo(FU);
            Assert.AreEqual(212.0,       inF.Value,            EPSILON);
            Assert.AreEqual("FAHRENHEIT", inF.Unit.GetUnitName());

            // Equals
            Assert.IsTrue(q.Equals(C(100.0)));
            Assert.IsTrue(q.Equals(F(212.0)));

            // Arithmetic blocked
            Assert.Throws<NotSupportedException>(() => q.Add(C(10.0)));
            Assert.Throws<NotSupportedException>(() => q.Subtract(C(10.0)));
            Assert.Throws<NotSupportedException>(() => q.Divide(C(10.0)));
        }
    }
}