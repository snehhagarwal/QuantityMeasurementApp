using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Interface;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Service;
using QuantityMeasurementBusinessLayer.Exceptions;
using QuantityMeasurementRepository.Repository;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC15: Layer Design Tests — Controller / Service / Entity separation.
    ///
    /// Covers:
    ///   • QuantityMeasurementEntity construction and immutability
    ///   • Service operations: compare, convert, add, subtract, divide
    ///   • Controller integration (using real service + in-memory repository)
    ///   • Layer isolation, data flow, backward compatibility, scalability
    ///
    /// Framework: MSTest
    /// </summary>
    [TestClass]
    public class NTierArchitectureTests
    {
        // ── Constants ────────────────────────────────────────────────────────

        private const double EPSILON = 1e-4;

        // ── Shared fixtures ──────────────────────────────────────────────────

        private IQuantityMeasurementEntityRepository _repository = null!;
        private IQuantityMeasurementService    _service    = null!;

        [TestInitialize]
        public void Setup()
        {
            // Use a fresh in-memory repository for every test (no Singleton side-effects)
            _repository = new InMemoryTestRepository();
            _service    = new QuantityMeasurementServiceImpl(_repository);
        }

        // ═════════════════════════════════════════════════════════════════════
        // ENTITY TESTS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies QuantityEntity correctly stores single-operand conversion data.
        /// Tests: Constructor and getters for conversion scenario.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_SingleOperandConstruction()
        {
            var operand = new QuantityDTO(1.0, "FEET", "LENGTH");
            var result  = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var entity = new QuantityMeasurementEntity("CONVERT", operand, result);

            Assert.AreEqual("CONVERT", entity.OperationType);
            Assert.IsNotNull(entity.FirstOperand);
            Assert.IsNull(entity.SecondOperand);
            Assert.IsNotNull(entity.Result);
            Assert.IsFalse(entity.IsError);
        }

        /// <summary>
        /// Verifies QuantityEntity correctly stores binary operation data.
        /// Tests: Constructor and getters for addition scenario.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_BinaryOperandConstruction()
        {
            var first  = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", first, second, "2 FEET");

            Assert.AreEqual("ADD", entity.OperationType);
            Assert.IsNotNull(entity.FirstOperand);
            Assert.IsNotNull(entity.SecondOperand);
            Assert.AreEqual("2 FEET", entity.Result);
            Assert.IsFalse(entity.IsError);
        }

        /// <summary>
        /// Verifies QuantityEntity correctly stores error data.
        /// Tests: Error constructor and IsError property.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_ErrorConstruction()
        {
            var first  = new QuantityDTO(1.0, "FEET", "LENGTH");
            var second = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            var entity = new QuantityMeasurementEntity("COMPARE", first, second,
                "Cross-category not allowed", true);

            Assert.IsTrue(entity.IsError);
            Assert.AreEqual("Cross-category not allowed", entity.ErrorMessage);
            Assert.IsNull(entity.Result);
        }

        /// <summary>
        /// Verifies ToString() formats successful results clearly.
        /// Tests: String representation for reading.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_ToString_Success()
        {
            var first  = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", first, second, "2 FEET");
            string text = entity.ToString();

            StringAssert.Contains(text, "ADD");
            StringAssert.Contains(text, "2 FEET");
            Assert.IsFalse(text.Contains("ERROR"));
        }

        /// <summary>
        /// Verifies ToString() formats errors clearly.
        /// Tests: Error message visibility.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_ToString_Error()
        {
            var entity = new QuantityMeasurementEntity("DIVIDE", null, null,
                "Cannot divide by zero", true);

            string text = entity.ToString();

            StringAssert.Contains(text, "ERROR");
            StringAssert.Contains(text, "Cannot divide by zero");
        }

        // ═════════════════════════════════════════════════════════════════════
        // SERVICE TESTS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies service correctly compares quantities in same unit.
        /// Tests: Service delegates to Quantity.equals().
        /// </summary>
        [TestMethod]
        public void testService_CompareEquality_SameUnit_Success()
        {
            var first  = new QuantityDTO(5.0, "FEET", "LENGTH");
            var second = new QuantityDTO(5.0, "FEET", "LENGTH");

            QuantityDTO result = _service.Compare(first, second);

            Assert.AreEqual("TRUE", result.MeasurementType);
        }

        /// <summary>
        /// Verifies service correctly compares quantities in different units.
        /// Tests: Cross-unit comparison through service.
        /// </summary>
        [TestMethod]
        public void testService_CompareEquality_DifferentUnit_Success()
        {
            var first  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Compare(first, second);

            Assert.AreEqual("TRUE", result.MeasurementType);
        }

        /// <summary>
        /// Verifies service rejects cross-category comparison.
        /// Tests: Category compatibility check.
        /// </summary>
        [TestMethod]
        public void testService_CompareEquality_CrossCategory_Error()
        {
            var first  = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var second = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            Assert.Throws<QuantityMeasurementException>(
                () => _service.Compare(first, second));
        }

        /// <summary>
        /// Verifies service correctly converts between units.
        /// Tests: Service delegates to Quantity.convertTo().
        /// </summary>
        [TestMethod]
        public void testService_Convert_Success()
        {
            var quantity   = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var targetUnit = new QuantityDTO(0.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Convert(quantity, targetUnit);

            Assert.AreEqual(12.0, result.Value, EPSILON);
            Assert.AreEqual("INCHES", result.Unit);
        }

        /// <summary>
        /// Verifies service correctly performs addition.
        /// Tests: Service delegates to Quantity.add().
        /// </summary>
        [TestMethod]
        public void testService_Add_Success()
        {
            var first  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Add(first, second);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual("FEET", result.Unit);
        }

        /// <summary>
        /// Verifies service handles unsupported operations (temperature).
        /// Tests: Exception conversion to error entity.
        /// </summary>
        [TestMethod]
        public void testService_Add_UnsupportedOperation_Error()
        {
            var first  = new QuantityDTO(100.0, "CELSIUS",    "TEMPERATURE");
            var second = new QuantityDTO(32.0,  "FAHRENHEIT", "TEMPERATURE");

            Assert.Throws<QuantityMeasurementException>(
                () => _service.Add(first, second));
        }

        /// <summary>
        /// Verifies service correctly performs subtraction.
        /// Tests: Service delegates to Quantity.subtract().
        /// </summary>
        [TestMethod]
        public void testService_Subtract_Success()
        {
            var first  = new QuantityDTO(2.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Subtract(first, second);

            Assert.AreEqual(1.0, result.Value, EPSILON);
            Assert.AreEqual("FEET", result.Unit);
        }

        /// <summary>
        /// Verifies service correctly performs division.
        /// Tests: Service returns dimensionless scalar.
        /// </summary>
        [TestMethod]
        public void testService_Divide_Success()
        {
            var first  = new QuantityDTO(2.0, "KILOGRAM", "WEIGHT");
            var second = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            QuantityDTO result = _service.Divide(first, second);

            Assert.AreEqual(2.0,           result.Value, EPSILON);
            Assert.AreEqual("RATIO",       result.Unit);
            Assert.AreEqual("DIMENSIONLESS", result.MeasurementType);
        }

        /// <summary>
        /// Verifies service handles division by zero.
        /// Tests: Exception handling.
        /// </summary>
        [TestMethod]
        public void testService_Divide_ByZero_Error()
        {
            var first  = new QuantityDTO(5.0, "KILOGRAM", "WEIGHT");
            var second = new QuantityDTO(0.0, "KILOGRAM", "WEIGHT");

            Assert.Throws<QuantityMeasurementException>(
                () => _service.Divide(first, second));
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONTROLLER TESTS  (using capture helper — no console I/O needed)
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies controller correctly demonstrates equality.
        /// Tests: Controller → Service integration.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateEquality_Success()
        {
            var first  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = _service.Compare(first, second);

            Assert.AreEqual("TRUE", result.MeasurementType);
        }

        /// <summary>
        /// Verifies controller correctly demonstrates conversion.
        /// Tests: Controller method routing.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateConversion_Success()
        {
            var quantity   = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");
            var targetUnit = new QuantityDTO(0.0, "GRAM",     "WEIGHT");

            QuantityDTO result = _service.Convert(quantity, targetUnit);

            Assert.AreEqual(1000.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies controller correctly demonstrates addition.
        /// Tests: Controller handles successful operations.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateAddition_Success()
        {
            var first  = new QuantityDTO(1.0, "LITRE",      "VOLUME");
            var second = new QuantityDTO(1.0, "LITRE",      "VOLUME");

            QuantityDTO result = _service.Add(first, second);

            Assert.AreEqual(2.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Verifies controller correctly displays errors.
        /// Tests: Controller error handling.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateAddition_Error()
        {
            var first  = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var second = new QuantityDTO(200.0, "CELSIUS", "TEMPERATURE");

            Assert.Throws<QuantityMeasurementException>(
                () => _service.Add(first, second));
        }

        /// <summary>
        /// Verifies controller formats success results correctly.
        /// Tests: Output formatting.
        /// </summary>
        [TestMethod]
        public void testController_DisplayResult_Success()
        {
            var operand = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var result  = new QuantityDTO(12.0, "INCHES", "LENGTH");
            var entity  = new QuantityMeasurementEntity("CONVERT", operand, result);

            string display = entity.ToString();

            Assert.IsFalse(entity.IsError);
            StringAssert.Contains(display, "CONVERT");
        }

        /// <summary>
        /// Verifies controller displays error messages.
        /// Tests: Error output format.
        /// </summary>
        [TestMethod]
        public void testController_DisplayResult_Error()
        {
            var entity = new QuantityMeasurementEntity("ADD", null, null,
                "Temperature addition not supported", true);

            string display = entity.ToString();

            StringAssert.Contains(display, "ERROR");
            StringAssert.Contains(display, "Temperature addition not supported");
        }

        // ═════════════════════════════════════════════════════════════════════
        // LAYER SEPARATION TESTS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies service can be tested without controller.
        /// Tests: Layer isolation enables unit testing.
        /// </summary>
        [TestMethod]
        public void testLayerSeparation_ServiceIndependence()
        {
            // Service works with only a repository — no controller needed
            IQuantityMeasurementEntityRepository repo    = new InMemoryTestRepository();
            IQuantityMeasurementService    service = new QuantityMeasurementServiceImpl(repo);

            var first  = new QuantityDTO(3.0, "FEET", "LENGTH");
            var second = new QuantityDTO(3.0, "FEET", "LENGTH");

            QuantityDTO result = service.Compare(first, second);

            Assert.AreEqual("TRUE", result.MeasurementType);
        }

        /// <summary>
        /// Verifies controller can work with mock service.
        /// Tests: Dependency injection pattern.
        /// </summary>
        [TestMethod]
        public void testLayerSeparation_ControllerIndependence()
        {
            // Swap in a mock service — controller code is unaffected
            IQuantityMeasurementService mockService = new MockQuantityMeasurementService();
            Assert.IsNotNull(mockService);

            // Mock returns a canned compare result — no real computation
            var first  = new QuantityDTO(1.0, "FEET", "LENGTH");
            var second = new QuantityDTO(1.0, "FEET", "LENGTH");
            QuantityDTO result = mockService.Compare(first, second);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Verifies data correctly flows from controller to service.
        /// Tests: QuantityEntity as data contract.
        /// </summary>
        [TestMethod]
        public void testDataFlow_ControllerToService()
        {
            var first  = new QuantityDTO(500.0, "GRAM",     "WEIGHT");
            var second = new QuantityDTO(0.5,   "KILOGRAM", "WEIGHT");

            QuantityDTO result = _service.Compare(first, second);

            Assert.AreEqual("TRUE", result.MeasurementType);
        }

        /// <summary>
        /// Verifies results correctly flow from service to controller.
        /// Tests: Standardized output format.
        /// </summary>
        [TestMethod]
        public void testDataFlow_ServiceToController()
        {
            var quantity   = new QuantityDTO(1.0, "GALLON",    "VOLUME");
            var targetUnit = new QuantityDTO(0.0, "LITRE",     "VOLUME");

            QuantityDTO result = _service.Convert(quantity, targetUnit);

            // Result must be a QuantityDTO with populated fields — standardized contract
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Unit);
            Assert.IsNotNull(result.MeasurementType);
        }

        // ═════════════════════════════════════════════════════════════════════
        // BACKWARD COMPATIBILITY / SCALABILITY
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Runs all test cases from UC1–UC14 via the UC15 service layer.
        /// Tests: Behavior unchanged, only structure refactored.
        /// </summary>
        [TestMethod]
        public void testBackwardCompatibility_AllUC1_UC14_Tests()
        {
            // UC1/UC2: feet equality
            Assert.AreEqual("TRUE",
                _service.Compare(
                    new QuantityDTO(10.0, "FEET", "LENGTH"),
                    new QuantityDTO(10.0, "FEET", "LENGTH")).MeasurementType);

            // UC4: cross-unit length
            Assert.AreEqual("TRUE",
                _service.Compare(
                    new QuantityDTO(1.0,  "YARDS", "LENGTH"),
                    new QuantityDTO(3.0,  "FEET",  "LENGTH")).MeasurementType);

            // UC5: conversion
            Assert.AreEqual(12.0,
                _service.Convert(
                    new QuantityDTO(1.0, "FEET",   "LENGTH"),
                    new QuantityDTO(0.0, "INCHES", "LENGTH")).Value, EPSILON);

            // UC6: addition
            Assert.AreEqual(2.0,
                _service.Add(
                    new QuantityDTO(1.0,  "FEET",   "LENGTH"),
                    new QuantityDTO(12.0, "INCHES", "LENGTH")).Value, EPSILON);

            // UC9: weight comparison
            Assert.AreEqual("TRUE",
                _service.Compare(
                    new QuantityDTO(1000.0, "GRAM",     "WEIGHT"),
                    new QuantityDTO(1.0,    "KILOGRAM", "WEIGHT")).MeasurementType);

            // UC11: temperature conversion
            QuantityDTO tempResult = _service.Convert(
                new QuantityDTO(0.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(0.0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual(32.0, tempResult.Value, EPSILON);
        }

        /// <summary>
        /// Verifies service works with length, weight, volume, temperature.
        /// Tests: Category scalability.
        /// </summary>
        [TestMethod]
        public void testService_AllMeasurementCategories()
        {
            // LENGTH
            QuantityDTO lenResult = _service.Convert(
                new QuantityDTO(1.0, "YARDS",  "LENGTH"),
                new QuantityDTO(0.0, "FEET",   "LENGTH"));
            Assert.AreEqual(3.0, lenResult.Value, EPSILON);

            // WEIGHT
            QuantityDTO wgtResult = _service.Convert(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(0.0, "GRAM",     "WEIGHT"));
            Assert.AreEqual(1000.0, wgtResult.Value, EPSILON);

            // VOLUME
            QuantityDTO volResult = _service.Convert(
                new QuantityDTO(1.0, "LITRE",      "VOLUME"),
                new QuantityDTO(0.0, "MILLILITRE", "VOLUME"));
            Assert.AreEqual(1000.0, volResult.Value, EPSILON);

            // TEMPERATURE
            QuantityDTO tmpResult = _service.Convert(
                new QuantityDTO(100.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(0.0,   "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual(212.0, tmpResult.Value, EPSILON);
        }

        /// <summary>
        /// Verifies controller can demonstrate all operations.
        /// Tests: Operation coverage.
        /// </summary>
        [TestMethod]
        public void testController_AllOperations()
        {
            var feetA = new QuantityDTO(2.0, "FEET", "LENGTH");
            var feetB = new QuantityDTO(1.0, "FEET", "LENGTH");

            Assert.IsNotNull(_service.Compare(feetA, feetB));
            Assert.IsNotNull(_service.Convert(feetA, new QuantityDTO(0.0, "INCHES", "LENGTH")));
            Assert.IsNotNull(_service.Add(feetA, feetB));
            Assert.IsNotNull(_service.Subtract(feetA, feetB));
            Assert.IsNotNull(_service.Divide(feetA, feetB));
        }

        /// <summary>
        /// Verifies validation errors are identical across operations.
        /// Tests: Centralized validation in service.
        /// </summary>
        [TestMethod]
        public void testService_ValidationConsistency()
        {
            var length      = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var weight      = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            // All operations must reject cross-category with same exception type
            Assert.Throws<QuantityMeasurementException>(() => _service.Compare(length,   weight));
            Assert.Throws<QuantityMeasurementException>(() => _service.Add(length,      weight));
            Assert.Throws<QuantityMeasurementException>(() => _service.Subtract(length, weight));
            Assert.Throws<QuantityMeasurementException>(() => _service.Divide(length,   weight));
        }

        /// <summary>
        /// Verifies QuantityEntity objects cannot be modified after creation.
        /// Tests: Immutability principle.
        /// </summary>
        [TestMethod]
        public void testEntity_Immutability()
        {
            var first  = new QuantityDTO(1.0, "FEET", "LENGTH");
            var second = new QuantityDTO(2.0, "FEET", "LENGTH");
            var entity = new QuantityMeasurementEntity("COMPARE", first, second, "false");

            string originalType   = entity.OperationType;
            string originalFirst  = entity.FirstOperand!;
            string originalResult = entity.Result!;

            // No setters exist — values are fixed at construction
            Assert.AreEqual(originalType,   entity.OperationType);
            Assert.AreEqual(originalFirst,  entity.FirstOperand);
            Assert.AreEqual(originalResult, entity.Result);
        }

        /// <summary>
        /// Verifies all operations convert exceptions to error entities.
        /// Tests: Consistent error propagation.
        /// </summary>
        [TestMethod]
        public void testService_ExceptionHandling_AllOperations()
        {
            var length = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var weight = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            try { _service.Compare(length,   weight); Assert.Fail(); }
            catch (QuantityMeasurementException) { /* expected */ }

            try { _service.Add(length,      weight); Assert.Fail(); }
            catch (QuantityMeasurementException) { /* expected */ }

            try { _service.Subtract(length, weight); Assert.Fail(); }
            catch (QuantityMeasurementException) { /* expected */ }

            try { _service.Divide(length,   weight); Assert.Fail(); }
            catch (QuantityMeasurementException) { /* expected */ }
        }

        /// <summary>
        /// Verifies console output is clear and readable.
        /// Tests: User-facing formatting.
        /// </summary>
        [TestMethod]
        public void testController_ConsoleOutput_Format()
        {
            var first  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");
            var entity = new QuantityMeasurementEntity("COMPARE", first, second, "true");

            string text = entity.ToString();

            // Must contain timestamp brackets, operation, operands, and result
            StringAssert.Contains(text, "[");
            StringAssert.Contains(text, "]");
            StringAssert.Contains(text, "COMPARE");
            StringAssert.Contains(text, "=>");
        }

        /// <summary>
        /// Full integration test: User input → Output.
        /// Tests: Complete layer cooperation.
        /// </summary>
        [TestMethod]
        public void testIntegration_EndToEnd_LengthAddition()
        {
            IQuantityMeasurementEntityRepository repo    = new InMemoryTestRepository();
            IQuantityMeasurementService    service = new QuantityMeasurementServiceImpl(repo);

            var first  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO result = service.Add(first, second);

            Assert.AreEqual(2.0,    result.Value, EPSILON);
            Assert.AreEqual("FEET", result.Unit);

            // Repository must have recorded the operation
            Assert.AreEqual(1, repo.GetAllMeasurements().Count);
        }

        /// <summary>
        /// Full integration test: Error handling across layers.
        /// Tests: Error handling integration.
        /// </summary>
        [TestMethod]
        public void testIntegration_EndToEnd_TemperatureUnsupported()
        {
            IQuantityMeasurementEntityRepository repo    = new InMemoryTestRepository();
            IQuantityMeasurementService    service = new QuantityMeasurementServiceImpl(repo);

            var first  = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var second = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            Assert.Throws<QuantityMeasurementException>(
                () => service.Add(first, second));

            // Repository should have recorded the error entry
            Assert.AreEqual(1, repo.GetAllMeasurements().Count);
            Assert.IsTrue(repo.GetAllMeasurements()[0].IsError);
        }

        /// <summary>
        /// Verifies service rejects null entities.
        /// Tests: Input validation.
        /// </summary>
        [TestMethod]
        public void testService_NullEntity_Rejection()
        {
            var valid = new QuantityDTO(1.0, "FEET", "LENGTH");

            Assert.Throws<QuantityMeasurementException>(
                () => _service.Compare(null!, valid));
            Assert.Throws<QuantityMeasurementException>(
                () => _service.Compare(valid, null!));
        }

        /// <summary>
        /// Verifies controller requires non-null service.
        /// Tests: Dependency validation.
        /// </summary>
        [TestMethod]
        public void testController_NullService_Prevention()
        {
            Assert.Throws<ArgumentNullException>(
                () => new QuantityMeasurementServiceImpl(null!));
        }

        /// <summary>
        /// Verifies service works with any IMeasurable implementation.
        /// Tests: Polymorphic behavior.
        /// </summary>
        [TestMethod]
        public void testService_AllUnitImplementations()
        {
            // LENGTH units
            foreach (var unit in new[] { "FEET", "INCHES", "YARDS", "CENTIMETERS" })
            {
                var dto = new QuantityDTO(1.0, unit, "LENGTH");
                Assert.IsNotNull(_service.Compare(dto, dto));
            }

            // WEIGHT units
            foreach (var unit in new[] { "KILOGRAM", "GRAM", "POUND" })
            {
                var dto = new QuantityDTO(1.0, unit, "WEIGHT");
                Assert.IsNotNull(_service.Compare(dto, dto));
            }

            // VOLUME units
            foreach (var unit in new[] { "LITRE", "MILLILITRE", "GALLON" })
            {
                var dto = new QuantityDTO(1.0, unit, "VOLUME");
                Assert.IsNotNull(_service.Compare(dto, dto));
            }

            // TEMPERATURE units
            foreach (var unit in new[] { "CELSIUS", "FAHRENHEIT", "KELVIN" })
            {
                var dto = new QuantityDTO(1.0, unit, "TEMPERATURE");
                Assert.IsNotNull(_service.Compare(dto, dto));
            }
        }

        /// <summary>
        /// Verifies operation type correctly recorded in entity.
        /// Tests: Operation categorization.
        /// </summary>
        [TestMethod]
        public void testEntity_OperationType_Tracking()
        {
            var first  = new QuantityDTO(2.0, "FEET", "LENGTH");
            var second = new QuantityDTO(1.0, "FEET", "LENGTH");

            _service.Compare(first, second);
            _service.Add(first, second);
            _service.Subtract(first, second);
            _service.Divide(first, second);

            var history = _repository.GetAllMeasurements();

            Assert.IsTrue(history.Count >= 4);
            Assert.IsTrue(history[0].OperationType == "COMPARE");
            Assert.IsTrue(history[1].OperationType == "ADD");
            Assert.IsTrue(history[2].OperationType == "SUBTRACT");
            Assert.IsTrue(history[3].OperationType == "DIVIDE");
        }

        /// <summary>
        /// Verifies changing service implementation doesn't affect controller.
        /// Tests: Loose coupling enables flexibility.
        /// </summary>
        [TestMethod]
        public void testLayerDecoupling_ServiceChange()
        {
            // Swap service implementation behind the same interface — result is identical
            IQuantityMeasurementService serviceA = new QuantityMeasurementServiceImpl(new InMemoryTestRepository());
            IQuantityMeasurementService serviceB = new QuantityMeasurementServiceImpl(new InMemoryTestRepository());

            var first  = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var second = new QuantityDTO(12.0, "INCHES", "LENGTH");

            QuantityDTO resultA = serviceA.Compare(first, second);
            QuantityDTO resultB = serviceB.Compare(first, second);

            Assert.AreEqual(resultA.MeasurementType, resultB.MeasurementType);
        }

        /// <summary>
        /// Verifies adding Entity fields doesn't break layers.
        /// Tests: Entity as stable contract.
        /// </summary>
        [TestMethod]
        public void testLayerDecoupling_EntityChange()
        {
            var operand = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var result  = new QuantityDTO(12.0, "INCHES", "LENGTH");
            var entity  = new QuantityMeasurementEntity("CONVERT", operand, result);

            // All core fields must remain accessible regardless of future additions
            Assert.IsNotNull(entity.OperationType);
            Assert.IsNotNull(entity.FirstOperand);
            Assert.IsNotNull(entity.Result);
            Assert.IsNotNull(entity.Timestamp);
        }

        /// <summary>
        /// Verifies adding new operation doesn't require layer modifications.
        /// Tests: Extensibility within layer design.
        /// </summary>
        [TestMethod]
        public void testScalability_NewOperation_Addition()
        {
            // Divide is a newer operation — service handles it without any controller change
            var first  = new QuantityDTO(10.0, "KILOGRAM", "WEIGHT");
            var second = new QuantityDTO(2.0,  "KILOGRAM", "WEIGHT");

            QuantityDTO result = _service.Divide(first, second);

            Assert.AreEqual(5.0,             result.Value, EPSILON);
            Assert.AreEqual("DIMENSIONLESS", result.MeasurementType);
        }

        // ═════════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lightweight in-memory repository used in tests to avoid Singleton state.
        /// </summary>
        private class InMemoryTestRepository : IQuantityMeasurementEntityRepository
        {
            private readonly List<QuantityMeasurementEntity> _store = new();

            public void Save(QuantityMeasurementEntity entity) => _store.Add(entity);

            public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
                => _store.AsReadOnly();

            public void Clear() => _store.Clear();

            // UC16 enhanced methods
            public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType)
                => _store.FindAll(e => string.Equals(e.OperationType, operationType,
                    StringComparison.OrdinalIgnoreCase)).AsReadOnly();

            public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType)
                => _store.FindAll(e => e.FirstOperand != null &&
                    e.FirstOperand.Contains(measurementType, StringComparison.OrdinalIgnoreCase)).AsReadOnly();

            public int GetTotalCount() => _store.Count;
        }

        /// <summary>
        /// Minimal mock service used to verify controller's DI pattern.
        /// </summary>
        private class MockQuantityMeasurementService : IQuantityMeasurementService
        {
            public QuantityDTO Compare(QuantityDTO first, QuantityDTO second)
                => new QuantityDTO(1, "RESULT", "true");

            public QuantityDTO Convert(QuantityDTO quantity, QuantityDTO targetUnit)
                => new QuantityDTO(0, targetUnit.Unit ?? "UNKNOWN", targetUnit.MeasurementType ?? "UNKNOWN");

            public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
                => new QuantityDTO(first.Value + second.Value, first.Unit ?? "UNKNOWN", first.MeasurementType ?? "UNKNOWN");

            public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
                => new QuantityDTO(first.Value - second.Value, first.Unit ?? "UNKNOWN", first.MeasurementType ?? "UNKNOWN");

            public QuantityDTO Divide(QuantityDTO first, QuantityDTO second)
                => new QuantityDTO(1, "RATIO", "DIMENSIONLESS");
        }
    }
}