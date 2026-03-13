using System;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementBusinessLayer.Unit;
using QuantityMeasurementRepository.Repository;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC15: Single service implementation for ALL quantity measurement operations.
    ///
    /// Implements IQuantityMeasurementService.
    /// Replaces the six separate service classes (LengthService, WeightService,
    /// VolumeService, TemperatureService, FeetService, InchesService).
    ///
    /// Design Principles followed:
    ///   SRP  — solely responsible for quantity measurement business logic.
    ///   OCP  — new units/categories added without modifying existing code.
    ///   DIP  — depends on IQuantityMeasurementRepository abstraction (injected).
    /// </summary>
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;

        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
        {
            _repository = repository
                ?? throw new System.ArgumentNullException(nameof(repository));
        }

        // ═════════════════════════════════════════════════════════════════════
        // COMPARE
        // ═════════════════════════════════════════════════════════════════════

        public QuantityDTO Compare(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "COMPARE");

                IMeasurable unitA = ResolveUnit(first);
                IMeasurable unitB = ResolveUnit(second);

                double baseA = unitA.ConvertToBaseUnit(first.Value);
                double baseB = unitB.ConvertToBaseUnit(second.Value);

                const double epsilon = 1e-4;
                bool equal = System.Math.Abs(baseA - baseB) < epsilon;

                var result = new QuantityDTO(equal ? 1 : 0, "RESULT", equal.ToString().ToLower());

                _repository.Save(new QuantityMeasurementEntity("COMPARE", first, second,
                    equal.ToString().ToLower()));

                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                SaveError("COMPARE", first, second, ex.Message);
                throw new QuantityMeasurementException("Compare operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONVERT
        // ═════════════════════════════════════════════════════════════════════

        public QuantityDTO Convert(QuantityDTO quantity, QuantityDTO targetUnit)
        {
            try
            {
                ValidateNotNull(quantity,   nameof(quantity));
                ValidateNotNull(targetUnit, nameof(targetUnit));
                ValidateSameCategory(quantity, targetUnit, "CONVERT");

                IMeasurable srcUnit = ResolveUnit(quantity);
                IMeasurable tgtUnit = ResolveUnit(targetUnit);

                double baseValue = srcUnit.ConvertToBaseUnit(quantity.Value);
                double converted = tgtUnit.ConvertFromBaseUnit(baseValue);
                double rounded   = System.Math.Round(converted, 4);

                var result = new QuantityDTO(rounded, tgtUnit.GetUnitName(), tgtUnit.GetMeasurementType());

                _repository.Save(new QuantityMeasurementEntity("CONVERT", quantity, result));

                return result;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                SaveError("CONVERT", quantity, targetUnit, ex.Message);
                throw new QuantityMeasurementException("Convert operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // ADD
        // ═════════════════════════════════════════════════════════════════════

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "ADD");

                IMeasurable unitA = ResolveUnit(first);
                unitA.ValidateOperationSupport("Add");

                IMeasurable unitB = ResolveUnit(second);

                double baseA   = unitA.ConvertToBaseUnit(first.Value);
                double baseB   = unitB.ConvertToBaseUnit(second.Value);
                double baseSum = baseA + baseB;
                double result  = System.Math.Round(unitA.ConvertFromBaseUnit(baseSum), 4);

                var resultDto = new QuantityDTO(result, unitA.GetUnitName(), unitA.GetMeasurementType());

                _repository.Save(new QuantityMeasurementEntity("ADD", first, second,
                    resultDto.ToString()));

                return resultDto;
            }
            catch (System.NotSupportedException ex)
            {
                SaveError("ADD", first, second, ex.Message);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                SaveError("ADD", first, second, ex.Message);
                throw new QuantityMeasurementException("Add operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // SUBTRACT
        // ═════════════════════════════════════════════════════════════════════

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "SUBTRACT");

                IMeasurable unitA = ResolveUnit(first);
                unitA.ValidateOperationSupport("Subtract");

                IMeasurable unitB = ResolveUnit(second);

                double baseA    = unitA.ConvertToBaseUnit(first.Value);
                double baseB    = unitB.ConvertToBaseUnit(second.Value);
                double baseDiff = baseA - baseB;
                double result   = System.Math.Round(unitA.ConvertFromBaseUnit(baseDiff), 4);

                var resultDto = new QuantityDTO(result, unitA.GetUnitName(), unitA.GetMeasurementType());

                _repository.Save(new QuantityMeasurementEntity("SUBTRACT", first, second,
                    resultDto.ToString()));

                return resultDto;
            }
            catch (System.NotSupportedException ex)
            {
                SaveError("SUBTRACT", first, second, ex.Message);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                SaveError("SUBTRACT", first, second, ex.Message);
                throw new QuantityMeasurementException("Subtract operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // DIVIDE
        // ═════════════════════════════════════════════════════════════════════

        public QuantityDTO Divide(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "DIVIDE");

                IMeasurable unitA = ResolveUnit(first);
                unitA.ValidateOperationSupport("Divide");

                IMeasurable unitB = ResolveUnit(second);

                double baseA = unitA.ConvertToBaseUnit(first.Value);
                double baseB = unitB.ConvertToBaseUnit(second.Value);

                if (System.Math.Abs(baseB) < 1e-12)
                    throw new System.ArithmeticException("Cannot divide by a zero quantity.");

                double ratio = System.Math.Round(baseA / baseB, 4);

                var resultDto = new QuantityDTO(ratio, "RATIO", "DIMENSIONLESS");

                _repository.Save(new QuantityMeasurementEntity("DIVIDE", first, second,
                    ratio.ToString("G")));

                return resultDto;
            }
            catch (System.NotSupportedException ex)
            {
                SaveError("DIVIDE", first, second, ex.Message);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (QuantityMeasurementException) { throw; }
            catch (System.Exception ex)
            {
                SaveError("DIVIDE", first, second, ex.Message);
                throw new QuantityMeasurementException("Divide operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ═════════════════════════════════════════════════════════════════════

        private static IMeasurable ResolveUnit(QuantityDTO dto)
        {
            string type = dto.MeasurementType?.ToUpper()
                ?? throw new QuantityMeasurementException("MeasurementType is required in QuantityDTO.");
            string unit = dto.Unit?.ToUpper()
                ?? throw new QuantityMeasurementException("Unit is required in QuantityDTO.");

            return type switch
            {
                "LENGTH" => unit switch
                {
                    "FEET"        => new LengthUnitMeasurable(LengthUnit.FEET),
                    "INCHES"      => new LengthUnitMeasurable(LengthUnit.INCHES),
                    "YARDS"       => new LengthUnitMeasurable(LengthUnit.YARDS),
                    "CENTIMETERS" => new LengthUnitMeasurable(LengthUnit.CENTIMETERS),
                    _ => throw new QuantityMeasurementException($"Unknown LENGTH unit: {unit}")
                },
                "WEIGHT" => unit switch
                {
                    "KILOGRAM" => new WeightUnitMeasurable(WeightUnit.KILOGRAM),
                    "GRAM"     => new WeightUnitMeasurable(WeightUnit.GRAM),
                    "POUND"    => new WeightUnitMeasurable(WeightUnit.POUND),
                    _ => throw new QuantityMeasurementException($"Unknown WEIGHT unit: {unit}")
                },
                "VOLUME" => unit switch
                {
                    "LITRE"      => new VolumeUnitMeasurable(VolumeUnit.LITRE),
                    "MILLILITRE" => new VolumeUnitMeasurable(VolumeUnit.MILLILITRE),
                    "GALLON"     => new VolumeUnitMeasurable(VolumeUnit.GALLON),
                    _ => throw new QuantityMeasurementException($"Unknown VOLUME unit: {unit}")
                },
                "TEMPERATURE" => unit switch
                {
                    "CELSIUS"    => new TemperatureUnitMeasurable(TemperatureUnit.CELSIUS),
                    "FAHRENHEIT" => new TemperatureUnitMeasurable(TemperatureUnit.FAHRENHEIT),
                    "KELVIN"     => new TemperatureUnitMeasurable(TemperatureUnit.KELVIN),
                    _ => throw new QuantityMeasurementException($"Unknown TEMPERATURE unit: {unit}")
                },
                _ => throw new QuantityMeasurementException($"Unknown MeasurementType: {type}")
            };
        }

        private static void ValidateNotNull(QuantityDTO dto, string name)
        {
            if (dto == null)
                throw new QuantityMeasurementException($"{name} QuantityDTO cannot be null.");
        }

        private static void ValidateSameCategory(QuantityDTO first, QuantityDTO second, string operation)
        {
            if (!string.Equals(first.MeasurementType, second.MeasurementType,
                               System.StringComparison.OrdinalIgnoreCase))
            {
                throw new QuantityMeasurementException(
                    $"Cross-category {operation} not allowed: " +
                    $"{first.MeasurementType} vs {second.MeasurementType}.");
            }
        }

        private void SaveError(string operation, QuantityDTO? first, QuantityDTO? second, string message)
        {
            try
            {
                _repository.Save(new QuantityMeasurementEntity(operation, first, second,
                    message, true));
            }
            catch { /* repository errors must not shadow the original exception */ }
        }
    }

    // ── Legacy compatibility services retained for existing tests ─────────────

    /// <summary>Legacy service retained for FeetTests.cs compatibility.</summary>
    public class FeetService
    {
        public bool AreEqualWithTolerance(QuantityMeasurementModel.Entities.Feet first,
                                          QuantityMeasurementModel.Entities.Feet second,
                                          double tolerance)
        {
            if (tolerance < 0)
                throw new System.ArgumentException("Tolerance cannot be negative.", nameof(tolerance));
            return System.Math.Abs(first.Value - second.Value) <= tolerance;
        }
    }

    /// <summary>Legacy service retained for InchesTest.cs compatibility.</summary>
    public class InchesService
    {
        public bool AreEqualWithTolerance(QuantityMeasurementModel.Entities.Inches first,
                                          QuantityMeasurementModel.Entities.Inches second,
                                          double tolerance)
        {
            if (tolerance < 0)
                throw new System.ArgumentException("Tolerance cannot be negative.", nameof(tolerance));
            return System.Math.Abs(first.Value - second.Value) <= tolerance;
        }
    }

    /// <summary>Legacy service retained for ExtendedUnitSupport.cs compatibility.</summary>
    public class LengthService
    {
        public bool AreEqualWithTolerance(QuantityMeasurementModel.Entities.Length first,
                                          QuantityMeasurementModel.Entities.Length second,
                                          double tolerance)
        {
            if (tolerance < 0)
                throw new System.ArgumentException("Tolerance cannot be negative.", nameof(tolerance));
            // Compare via base unit (feet) using LengthUnitExtensions class
            double baseA = new LengthUnitExtensions(first.Unit).ConvertToBaseUnit(first.Value);
            double baseB = new LengthUnitExtensions(second.Unit).ConvertToBaseUnit(second.Value);
            return System.Math.Abs(baseA - baseB) <= tolerance;
        }
    }
}
