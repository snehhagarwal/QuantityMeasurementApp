using System;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Exception;
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
    ///
    /// Broad steps followed in every operation:
    ///   1. Accept QuantityDTO input.
    ///   2. Extract IMeasurable units from QuantityDTO (via ResolveUnit helper).
    ///   3. Validate operands (null check, cross-category check).
    ///   4. Perform business logic using Quantity&lt;IMeasurable&gt; internally.
    ///   5. Handle exceptions → wrap in QuantityMeasurementException.
    ///   6. Create QuantityMeasurementEntity and save to IQuantityMeasurementRepository.
    ///   7. Return standardized QuantityDTO result.
    /// </summary>
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        // ── Dependency — injected via constructor (Dependency Injection) ───────

        private readonly IQuantityMeasurementRepository _repository;

        /// <summary>Constructor injection — preferred DI pattern.</summary>
        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
        {
            _repository = repository
                ?? throw new System.ArgumentNullException(nameof(repository));
        }

        // ═════════════════════════════════════════════════════════════════════
        // COMPARE
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Compares two quantities for equality.
        /// Both must belong to the same measurement category.
        /// </summary>
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
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                SaveError("COMPARE", first, second, ex.Message);
                throw new QuantityMeasurementException("Compare operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONVERT
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Converts a quantity to the unit specified in targetUnit.
        /// targetUnit.Unit must be a valid unit name in the same category.
        /// </summary>
        public QuantityDTO Convert(QuantityDTO quantity, QuantityDTO targetUnit)
        {
            try
            {
                ValidateNotNull(quantity,   nameof(quantity));
                ValidateNotNull(targetUnit, nameof(targetUnit));
                ValidateSameCategory(quantity, targetUnit, "CONVERT");

                IMeasurable srcUnit = ResolveUnit(quantity);
                IMeasurable tgtUnit = ResolveUnit(targetUnit);

                double baseValue  = srcUnit.ConvertToBaseUnit(quantity.Value);
                double converted  = tgtUnit.ConvertFromBaseUnit(baseValue);
                double rounded    = System.Math.Round(converted, 4);

                var result = new QuantityDTO(rounded, tgtUnit.GetUnitName(), tgtUnit.GetMeasurementType());

                _repository.Save(new QuantityMeasurementEntity("CONVERT", quantity, result));

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                SaveError("CONVERT", quantity, targetUnit, ex.Message);
                throw new QuantityMeasurementException("Convert operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // ADD
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Adds two quantities. Result is in the first operand's unit.
        /// Throws QuantityMeasurementException for Temperature (unsupported arithmetic).
        /// </summary>
        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "ADD");

                IMeasurable unitA = ResolveUnit(first);
                unitA.ValidateOperationSupport("Add");   // throws for Temperature

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
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                SaveError("ADD", first, second, ex.Message);
                throw new QuantityMeasurementException("Add operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // SUBTRACT
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Subtracts second from first. Result is in the first operand's unit.
        /// Throws QuantityMeasurementException for Temperature (unsupported arithmetic).
        /// </summary>
        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "SUBTRACT");

                IMeasurable unitA = ResolveUnit(first);
                unitA.ValidateOperationSupport("Subtract");   // throws for Temperature

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
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                SaveError("SUBTRACT", first, second, ex.Message);
                throw new QuantityMeasurementException("Subtract operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // DIVIDE
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Divides first by second. Returns dimensionless ratio as QuantityDTO.
        /// Throws QuantityMeasurementException for Temperature or division by zero.
        /// </summary>
        public QuantityDTO Divide(QuantityDTO first, QuantityDTO second)
        {
            try
            {
                ValidateNotNull(first,  nameof(first));
                ValidateNotNull(second, nameof(second));
                ValidateSameCategory(first, second, "DIVIDE");

                IMeasurable unitA = ResolveUnit(first);
                unitA.ValidateOperationSupport("Divide");   // throws for Temperature

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
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                SaveError("DIVIDE", first, second, ex.Message);
                throw new QuantityMeasurementException("Divide operation failed: " + ex.Message, ex);
            }
        }

        // ═════════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ═════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Resolves a QuantityDTO's unit name and measurement type to a concrete IMeasurable.
        /// This is the DTO → IMeasurable mapping used in all service methods.
        /// </summary>
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

        /// <summary>Validates that neither operand is null.</summary>
        private static void ValidateNotNull(QuantityDTO dto, string name)
        {
            if (dto == null)
                throw new QuantityMeasurementException($"{name} QuantityDTO cannot be null.");
        }

        /// <summary>
        /// Validates both DTOs belong to the same measurement category.
        /// Prevents cross-category operations such as LENGTH + WEIGHT.
        /// </summary>
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

        /// <summary>Saves an error entity to the repository.</summary>
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
            return System.Math.Abs(first.ToBaseUnit() - second.ToBaseUnit()) <= tolerance;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // Supporting types — consolidated from individual files
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>UC10/UC12/UC13/UC14: Generic Quantity class for any IMeasurable unit.</summary>
    public class Quantity<TUnit> where TUnit : IMeasurable
    {
        public double Value { get; }
        public TUnit Unit { get; }

        public Quantity(double value, TUnit unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null");
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            if (Math.Abs(value) > 10_000_000)
                throw new ArgumentException("Value too large. Invalid measurement.");
            Value = value;
            Unit  = unit;
        }

        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);

        public Quantity<TUnit> ConvertTo(TUnit targetUnit, int decimalPlaces = 2)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");
            double baseVal  = Unit.ConvertToBaseUnit(Value);
            double converted = targetUnit.ConvertFromBaseUnit(baseVal);
            double rounded   = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        private enum ArithmeticOperation { Add, Subtract, Divide }

        private static void ValidateArithmeticOperands(
            Quantity<TUnit> self, Quantity<TUnit> other,
            TUnit? targetUnit, bool targetUnitRequired)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Other quantity cannot be null");
            if (targetUnitRequired && targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");
            if (!double.IsFinite(self.Value))
                throw new ArgumentException("This quantity value must be finite");
            if (!double.IsFinite(other.Value))
                throw new ArgumentException("Other quantity value must be finite");
        }

        private double PerformBaseArithmetic(Quantity<TUnit> other, ArithmeticOperation operation)
        {
            this.Unit.ValidateOperationSupport(operation.ToString());
            double a = this.ToBaseUnit();
            double b = other.ToBaseUnit();
            return operation switch
            {
                ArithmeticOperation.Add      => a + b,
                ArithmeticOperation.Subtract => a - b,
                ArithmeticOperation.Divide   => b == 0.0
                    ? throw new ArithmeticException("Cannot divide by a zero quantity")
                    : a / b,
                _ => throw new InvalidOperationException($"Unsupported operation: {operation}")
            };
        }

        private Quantity<TUnit> BuildResult(double baseResult, TUnit targetUnit, int decimalPlaces)
        {
            double converted = targetUnit.ConvertFromBaseUnit(baseResult);
            double rounded   = Math.Round(converted, decimalPlaces);
            return new Quantity<TUnit>(rounded, targetUnit);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Add), this.Unit, decimalPlaces);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, targetUnit, targetUnitRequired: true);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Add), targetUnit, decimalPlaces);
        }

        public static Quantity<TUnit> Add(Quantity<TUnit> first, Quantity<TUnit> second,
                                          TUnit resultUnit, int decimalPlaces = 2)
        {
            if (first      == null) throw new ArgumentNullException(nameof(first));
            if (second     == null) throw new ArgumentNullException(nameof(second));
            if (resultUnit == null) throw new ArgumentNullException(nameof(resultUnit));
            return first.Add(second, resultUnit, decimalPlaces);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Subtract), this.Unit, decimalPlaces);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit targetUnit, int decimalPlaces = 2)
        {
            ValidateArithmeticOperands(this, other, targetUnit, targetUnitRequired: true);
            return BuildResult(PerformBaseArithmetic(other, ArithmeticOperation.Subtract), targetUnit, decimalPlaces);
        }

        public double Divide(Quantity<TUnit> other)
        {
            ValidateArithmeticOperands(this, other, default, targetUnitRequired: false);
            return PerformBaseArithmetic(other, ArithmeticOperation.Divide);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            var other = (Quantity<TUnit>)obj;
            if (Unit.GetType() != other.Unit.GetType()) return false;
            const double epsilon = 1e-9;
            return Math.Abs(ToBaseUnit() - other.ToBaseUnit()) < epsilon;
        }

        public override int GetHashCode() => ToBaseUnit().GetHashCode();
        public override string ToString()  => $"{Value:F2} {Unit.GetUnitName()}";
    }

    /// <summary>UC15: Internal model wrapping an IMeasurable unit with its value.</summary>
    public class QuantityModel<U> where U : IMeasurable
    {
        public double Value { get; }
        public U      Unit  { get; }

        public QuantityModel(double value, U unit) { Value = value; Unit = unit; }

        public double ToBaseUnit() => Unit.ConvertToBaseUnit(Value);
        public override string ToString() => $"{Value:G} {Unit.GetUnitName()}";
    }

    public readonly struct LengthUnitMeasurable : IMeasurable
    {
        public LengthUnit Unit { get; }
        public LengthUnitMeasurable(LengthUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => Unit.ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "LENGTH";

        public override bool Equals(object? obj) => obj is LengthUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

    public readonly struct WeightUnitMeasurable : IMeasurable
    {
        public WeightUnit Unit { get; }
        public WeightUnitMeasurable(WeightUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => Unit.ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "WEIGHT";

        public override bool Equals(object? obj) => obj is WeightUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

    public readonly struct VolumeUnitMeasurable : IMeasurable
    {
        public VolumeUnit Unit { get; }
        public VolumeUnitMeasurable(VolumeUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => Unit.ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "VOLUME";

        public override bool Equals(object? obj) => obj is VolumeUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

    public readonly struct TemperatureUnitMeasurable : IMeasurable
    {
        public TemperatureUnit Unit { get; }
        private static readonly IMeasurable.SupportsArithmetic supportsArithmetic = () => false;

        public TemperatureUnitMeasurable(TemperatureUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => Unit.GetConversionFactor();
        public double ConvertToBaseUnit(double value) => Unit.ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => Unit.ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "TEMPERATURE";

        public bool SupportsArithmeticOps() => supportsArithmetic();
        public void ValidateOperationSupport(string operation)
            => throw new NotSupportedException($"Temperature does not support {operation}.");

        public override bool Equals(object? obj) => obj is TemperatureUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

}