# Quantity Measurement Application

A progressive .NET 10 console application for performing measurement equality, conversion, and arithmetic across Length, Weight, Volume, and Temperature categories. The system grows incrementally through fifteen use cases — from a simple feet comparison to a fully generic, extensible arithmetic engine built on the `IMeasurable` interface and `Quantity<TUnit>`, culminating in a complete N-Tier architecture with controller, service, repository, and model layers powered by a unified `QuantityMeasurementServiceImpl`.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Use Cases](#use-cases)
- [Conversion Reference](#conversion-reference)
- [Console Menu](#console-menu)
- [Test Summary](#test-summary)
- [Build and Run](#build-and-run)
- [Design Principles](#design-principles)

---

## Overview

| Metric | Value |
|---|---|
| Framework | .NET 10 |
| Use Cases | UC1 – UC15 |
| Measurement Categories | Length, Weight, Volume, Temperature |
| Supported Units | 13 across 4 categories |
| Operations | Equality, Conversion, Add, Subtract, Divide |
| Total Tests | 433 |
| Test Framework | MSTest 4.x |

---

## Architecture

UC15 replaces the flat single-project structure with a clean four-project N-Tier solution. Each project has a single responsibility and depends only on the layer below it.

```
┌──────────────────────────────────────────────────────────────────────┐
│  QuantityMeasurementApp  (Presentation / Entry Point)                │
│  Program.cs  QuantityMeasurementController  IQuantityMeasurementApp  │
├──────────────────────────────────────────────────────────────────────┤
│  QuantityMeasurementBusinessLayer                                    │
│  IQuantityMeasurementService   QuantityMeasurementServiceImpl        │
│  IMeasurable   Quantity<TUnit>   *UnitMeasurable (×4)                │
│  LengthUnitExtensions  WeightUnitExtensions                          │
│  VolumeUnitExtensions  TemperatureUnitExtensions                     │
│  QuantityMeasurementException                                        │
├──────────────────────────────────────────────────────────────────────┤
│  QuantityMeasurementRepository                                       │
│  IQuantityMeasurementRepository  QuantityMeasurementCacheRepository  │
├──────────────────────────────────────────────────────────────────────┤
│  QuantityMeasurementModel                                            │
│  QuantityDTO  QuantityMeasurementEntity                              │
│  LengthUnit  WeightUnit  VolumeUnit  TemperatureUnit                 │
│  Feet  Inches  Length  Weight                                        │
└──────────────────────────────────────────────────────────────────────┘
```

**Dependency direction:** App → BusinessLayer → Repository → Model

Each layer communicates through interfaces — `IQuantityMeasurementService` between App and BLL, `IQuantityMeasurementRepository` between BLL and Repository. This makes the full stack ready for ASP.NET Controller migration and database swapping without modifying any business logic.

---

## Project Structure

```
QuantityMeasurementApp/
│
├── QuantityMeasurementApp/               ← Entry point (Exe)
│   ├── Controller/
│   │   └── QuantityMeasurementController.cs
│   ├── Interface/
│   │   └── IQuantityMeasurementApp.cs
│   └── Program.cs
│
├── QuantityMeasurementBusinessLayer/     ← Business logic
│   ├── Interface/
│   │   ├── IMeasurable.cs
│   │   └── IQuantityMeasurementService.cs
│   ├── Service/
│   │   ├── Quantity.cs
│   │   ├── QuantityMeasurementServiceImpl.cs
│   │   └── UnitMeasurables.cs
│   ├── Unit/
│   │   ├── LengthUnitExtensions.cs
│   │   ├── WeightUnitExtensions.cs
│   │   ├── VolumeUnitExtensions.cs
│   │   └── TemperatureUnitExtensions.cs
│   └── Exception/
│       └── QuantityMeasurementException.cs
│
├── QuantityMeasurementRepository/        ← Persistence layer
│   ├── Interface/
│   │   └── IQuantityMeasurementRepository.cs
│   └── Repository/
│       └── QuantityMeasurementCacheRepository.cs
│
├── QuantityMeasurementModel/             ← Pure data model
│   ├── Dto/
│   │   ├── QuantityDTO.cs
│   │   └── QuantityMeasurementEntity.cs
│   └── Entities/
│       ├── Feet.cs
│       ├── Inches.cs
│       ├── Length.cs
│       ├── Weight.cs
│       ├── LengthUnit.cs
│       ├── WeightUnit.cs
│       ├── VolumeUnit.cs
│       └── TemperatureUnit.cs
│
└── QuantityMeasurementApp.Tests/         ← All tests
    ├── FeetTests.cs
    ├── InchesTest.cs
    ├── QuantityTests.cs
    ├── ExtendedUnitSupport.cs
    ├── LengthConversionTests.cs
    ├── LengthAdditionTests.cs
    ├── LengthAdditionTargetUnitTests.cs
    ├── RefactoredDesignTests.cs
    ├── WeightTests.cs
    ├── GenericQuantityTests.cs
    ├── VolumeTests.cs
    ├── SubtractionAndDivisionTests.cs
    ├── CentralizedArithmeticLogicTests.cs
    ├── TemperatureMeasurementTests.cs
    └── NTierArchitectureTests.cs
```

---

## Use Cases

### UC1 — Feet Equality

Introduces the `Feet` entity class with value-based equality. The foundation for all future measurements.

```csharp
var a = new Feet(1.0);
var b = new Feet(1.0);
bool equal = a.Equals(b); // true
```

Validates against `NaN`, `Infinity`, negative values, and values exceeding the upper bound. Equality is reflexive, symmetric, and transitive.

---

### UC2 — Inches Equality

Adds an independent `Inches` entity following the same equality contract as `Feet`.

```csharp
var a = new Inches(12.0);
var b = new Inches(12.0);
bool equal = a.Equals(b); // true
```

---

### UC3 — Generic Length Equality

Replaces the `Feet` and `Inches` duplication with a single reusable `Length` class backed by the `LengthUnit` enum.

```csharp
var feet   = new Length(1.0,  LengthUnit.FEET);
var inches = new Length(12.0, LengthUnit.INCHES);
bool equal = feet.Equals(inches); // true — same physical length
```

Cross-unit equality is performed by converting both values to the base unit (FEET) before comparison.

---

### UC4 — Extended Unit Support

Adds `YARDS` and `CENTIMETERS` to `LengthUnit`. All four units participate in cross-unit equality.

```csharp
var yard = new Length(1.0, LengthUnit.YARDS);
var feet = new Length(3.0, LengthUnit.FEET);
bool equal = yard.Equals(feet); // true
```

| Unit | Relation to Base (FEET) |
|---|---|
| FEET | 1 ft = 1 ft |
| INCHES | 12 in = 1 ft |
| YARDS | 1 yd = 3 ft |
| CENTIMETERS | 30.48 cm = 1 ft |

---

### UC5 — Unit Conversion

Adds `ConvertTo()` to the `Length` class. Returns a new `Length` in the target unit.

```csharp
var feet   = new Length(1.0, LengthUnit.FEET);
var inches = feet.ConvertTo(LengthUnit.INCHES);  // Length(12.0, INCHES)
var yards  = feet.ConvertTo(LengthUnit.YARDS);   // Length(0.33, YARDS)
```

Conversion is bidirectional and precise to two decimal places by default.

---

### UC6 — Addition of Lengths

Adds `Add()` to the `Length` class. The result is expressed in the first operand's unit.

```csharp
var feet   = new Length(1.0,  LengthUnit.FEET);
var inches = new Length(12.0, LengthUnit.INCHES);
var sum    = feet.Add(inches); // Length(2.0, FEET)
```

| Operation | Result |
|---|---|
| 1 ft + 2 ft | 3.0 FEET |
| 1 ft + 12 in | 2.0 FEET |
| 12 in + 1 ft | 24.0 INCHES |

---

### UC7 — Addition with Explicit Target Unit

Overloads `Add()` to accept a `targetUnit` parameter, giving the caller control over the result unit.

```csharp
var sum = feet.Add(inches, LengthUnit.YARDS); // Length(0.67, YARDS)
```

| Operation | Target | Result |
|---|---|---|
| 1 ft + 12 in | FEET | 2.0 FEET |
| 1 ft + 12 in | INCHES | 24.0 INCHES |
| 1 ft + 12 in | YARDS | 0.67 YARDS |

---

### UC8 — Length Unit Refactoring

Moves conversion responsibility from the `Length` class into `LengthUnit` itself via C# extension methods (`LengthUnitExtensions`). Each unit now owns its own conversion logic — a Single Responsibility improvement.

```csharp
double feet = LengthUnit.INCHES.ConvertToBaseUnit(12.0);  // 1.0
double inch = LengthUnit.FEET.ConvertFromBaseUnit(1.0);   // 12.0
```

`LengthUnitMeasurable` wraps the enum as an `IMeasurable` struct, bridging the refactored enum to the generic `Quantity<TUnit>` class introduced in UC10.

---

### UC9 — Weight Measurement

Introduces the `Weight` entity with `WeightUnit` enum and its own `*Extensions` wrapper. Base unit is KILOGRAM.

```csharp
var kg   = new Weight(1.0,    WeightUnit.KILOGRAM);
var gram = new Weight(1000.0, WeightUnit.GRAM);

bool equal = kg.Equals(gram);  // true
var  sum   = kg.Add(gram);     // Weight(2.0, KILOGRAM)
```

| Unit | Conversion to KILOGRAM |
|---|---|
| KILOGRAM | × 1.0 |
| GRAM | × 0.001 |
| POUND | × 0.453592 |

Supports: equality, `ConvertTo()`, `Add()` with implicit and explicit target unit.

---

### UC10 — Generic Quantity Class

Introduces `IMeasurable` and `Quantity<TUnit>` — a single generic class that replaces the need for separate per-category classes for equality, conversion, and addition.

```csharp
public interface IMeasurable
{
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();
}
```

```csharp
var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

bool equal = feet.Equals(inches);                                          // true
var  sum   = feet.Add(inches);                                             // Quantity(2.0, FEET)
var  conv  = feet.ConvertTo(new LengthUnitMeasurable(LengthUnit.INCHES)); // Quantity(12.0, INCHES)
```

Cross-category operations are prevented at compile time by the generic type parameter.

---

### UC11 — Volume Measurement

Extends the system with `VolumeUnit`, `VolumeUnitExtensions`, and `VolumeUnitMeasurable`. Plugs directly into `Quantity<TUnit>` — zero changes to the generic infrastructure. Base unit is LITRE.

```csharp
var litre = new Quantity<VolumeUnitMeasurable>(1.0,    new VolumeUnitMeasurable(VolumeUnit.LITRE));
var ml    = new Quantity<VolumeUnitMeasurable>(1000.0, new VolumeUnitMeasurable(VolumeUnit.MILLILITRE));

bool equal = litre.Equals(ml); // true
var  sum   = litre.Add(ml);    // Quantity(2.0, LITRE)
```

| Unit | Conversion to LITRE |
|---|---|
| LITRE | × 1.0 |
| MILLILITRE | × 0.001 |
| GALLON (US) | × 3.78541 |

---

### UC12 — Subtraction and Division

Adds two new arithmetic operations to `Quantity<TUnit>`. No new unit categories — these operations extend the existing generic class and work across Length, Weight, and Volume automatically.

**Subtraction** returns a new `Quantity<TUnit>` — same design as `Add`.

```csharp
var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
var b = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.INCHES));

var diff         = a.Subtract(b);                                                 // Quantity(9.5, FEET)
var diffInInches = a.Subtract(b, new LengthUnitMeasurable(LengthUnit.INCHES));   // Quantity(114.0, INCHES)
```

**Division** returns a dimensionless `double` — the ratio between two quantities.

```csharp
var x = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
var y = new Quantity<LengthUnitMeasurable>(2.0,  new LengthUnitMeasurable(LengthUnit.FEET));

double ratio = x.Divide(y); // 5.0
```

| Operation | Result |
|---|---|
| 10 ft − 6 in | 9.5 FEET |
| 5 L − 500 mL | 4.5 LITRE |
| 10 ft ÷ 2 ft | 5.0 (scalar) |
| 1 L ÷ 500 mL | 2.0 (scalar) |
| anything ÷ 0 | `ArithmeticException` |

---

### UC13 — Centralized Arithmetic Logic (DRY Refactor)

A pure internal refactor of `Quantity<TUnit>` — no new features, no new units, no API changes. Eliminates code duplication across `Add`, `Subtract`, and `Divide` by extracting shared arithmetic logic into centralized helpers.

```csharp
private enum ArithmeticOperation { Add, Subtract, Divide }

private double PerformBaseArithmetic(Quantity<TUnit> other, ArithmeticOperation operation)
{
    ValidateArithmeticOperands(this, other);
    return operation switch
    {
        ArithmeticOperation.Add      => ToBaseUnit() + other.ToBaseUnit(),
        ArithmeticOperation.Subtract => ToBaseUnit() - other.ToBaseUnit(),
        ArithmeticOperation.Divide   => ToBaseUnit() / other.ToBaseUnit(),
        _ => throw new ArgumentException("Unknown operation")
    };
}
```

All existing tests pass without modification — the refactor is fully transparent to callers.

---

### UC14 — Temperature Measurement

Introduces `TemperatureUnit` (CELSIUS, FAHRENHEIT, KELVIN) as the fourth measurement category. Temperature supports equality and conversion but **not arithmetic** — adding or dividing temperatures produces physically meaningless results.

**Selective arithmetic via IMeasurable default methods:**

```csharp
public interface IMeasurable
{
    // UC14: default methods — existing units inherit these unchanged
    bool SupportsArithmeticOps()             => true;
    void ValidateOperationSupport(string op) { }  // no-op default
}
```

`TemperatureUnitMeasurable` overrides both to block arithmetic. All existing units (`LengthUnitMeasurable`, `WeightUnitMeasurable`, `VolumeUnitMeasurable`) inherit the default `true` — **zero changes required**.

**Non-linear conversion via lambda expressions:**

```csharp
private static readonly Func<double, double> CelsiusToFahrenheit = c => (c * 9.0 / 5.0) + 32.0;
private static readonly Func<double, double> FahrenheitToCelsius = f => (f - 32.0) * 5.0 / 9.0;
private static readonly Func<double, double> CelsiusToKelvin     = c => c + 273.15;
private static readonly Func<double, double> KelvinToCelsius     = k => k - 273.15;
```

```csharp
// Equality and conversion — fully supported
new Quantity<TemperatureUnitMeasurable>(0.0,   CELSIUS).Equals(
new Quantity<TemperatureUnitMeasurable>(32.0,  FAHRENHEIT))     // true

// Arithmetic — throws NotSupportedException
new Quantity<TemperatureUnitMeasurable>(100.0, CELSIUS).Add(
new Quantity<TemperatureUnitMeasurable>(50.0,  CELSIUS))        // NotSupportedException
```

**Temperature conversion formulas:**

| From | To | Formula |
|---|---|---|
| CELSIUS | FAHRENHEIT | `(C × 9/5) + 32` |
| FAHRENHEIT | CELSIUS | `(F − 32) × 5/9` |
| CELSIUS | KELVIN | `C + 273.15` |
| KELVIN | CELSIUS | `K − 273.15` |

---

### UC15 — N-Tier Architecture

Restructures the application from a single-project flat design into a clean **four-project N-Tier solution**. The `Quantity<TUnit>` engine from UC14 becomes the internal implementation detail of a unified service layer. All user-facing operations are standardized around `QuantityDTO` — a simple data transfer object that decouples the presentation layer from measurement internals entirely.

**Key structural changes from UC14:**

The six separate service interfaces (`ILengthService`, `IWeightService`, `IVolumeService`, `ITemperatureService`, `IFeetService`, `IInchesService`) are **replaced by a single** `IQuantityMeasurementService` with five operations. The per-category `*Presentation` and `*Repository` classes are replaced by `QuantityMeasurementController`, `QuantityMeasurementServiceImpl`, and `QuantityMeasurementCacheRepository`.

**IQuantityMeasurementService — the single service contract:**

```csharp
public interface IQuantityMeasurementService
{
    QuantityDTO Compare(QuantityDTO first, QuantityDTO second);
    QuantityDTO Convert(QuantityDTO quantity, QuantityDTO targetUnit);
    QuantityDTO Add(QuantityDTO first, QuantityDTO second);
    QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);
    QuantityDTO Divide(QuantityDTO first, QuantityDTO second);
}
```

**QuantityDTO — the layer boundary object:**

```csharp
// All operations use the same DTO contract
var feet   = new QuantityDTO(1.0,  "FEET",   "LENGTH");
var inches = new QuantityDTO(12.0, "INCHES", "LENGTH");

QuantityDTO result = service.Compare(feet, inches);
// result.MeasurementType == "TRUE"
// result.Value           == 1

QuantityDTO converted = service.Convert(feet, new QuantityDTO(0, "INCHES", "LENGTH"));
// converted.Value == 12.0
// converted.Unit  == "INCHES"

QuantityDTO sum = service.Add(feet, inches);
// sum.Value == 2.0
// sum.Unit  == "FEET"

QuantityDTO ratio = service.Divide(feet, new QuantityDTO(0.5, "FEET", "LENGTH"));
// ratio.Value           == 2.0
// ratio.MeasurementType == "DIMENSIONLESS"
```

**QuantityMeasurementServiceImpl internals:**

The service maps `QuantityDTO` inputs to the appropriate `*UnitMeasurable` struct via a `ResolveUnit` helper, performs business logic using the existing `Quantity<TUnit>` infrastructure from UC10–UC14, and records every operation as a `QuantityMeasurementEntity` in the repository before returning a `QuantityDTO` result. Errors are caught, wrapped in `QuantityMeasurementException`, and saved as error entities — so the repository provides a complete audit trail including failures.

```csharp
// Service resolves DTO → IMeasurable internally
private static IMeasurable ResolveUnit(QuantityDTO dto) => dto.MeasurementType switch
{
    "LENGTH"      => new LengthUnitMeasurable(Enum.Parse<LengthUnit>(dto.Unit)),
    "WEIGHT"      => new WeightUnitMeasurable(Enum.Parse<WeightUnit>(dto.Unit)),
    "VOLUME"      => new VolumeUnitMeasurable(Enum.Parse<VolumeUnit>(dto.Unit)),
    "TEMPERATURE" => new TemperatureUnitMeasurable(Enum.Parse<TemperatureUnit>(dto.Unit)),
    _             => throw new QuantityMeasurementException($"Unknown type: {dto.MeasurementType}")
};
```

**IQuantityMeasurementRepository — persistence contract:**

```csharp
public interface IQuantityMeasurementRepository
{
    void Save(QuantityMeasurementEntity entity);
    IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
    void Clear();
}
```

`QuantityMeasurementCacheRepository` implements this as a thread-safe singleton with in-memory list and disk persistence via append-mode text file — mirroring the Java `AppendableObjectOutputStream` pattern.

**QuantityMeasurementEntity — the audit record:**

Every operation saves one immutable entity recording the operation type, both operands, the result, any error message, and a timestamp. The controller's "Operation History" menu reads these directly from the repository.

```csharp
// Successful operation
new QuantityMeasurementEntity("ADD", firstDto, secondDto, "2 FEET");

// Error case
new QuantityMeasurementEntity("ADD", firstDto, secondDto, "Temperature does not support Add.", isError: true);
```

**Controller — presentation layer:**

`QuantityMeasurementController` implements `IQuantityMeasurementApp` and handles all console I/O. It accepts two DI constructors — a default that wires itself internally (for `Program.cs`) and an injected constructor (for tests and future ASP.NET migration). The five `PerformXxx` methods map directly to REST `POST` endpoints:

```
POST /api/quantity/compare
POST /api/quantity/convert
POST /api/quantity/add
POST /api/quantity/subtract
POST /api/quantity/divide
```

**Backward compatibility:**

All UC1–UC14 test behaviour is preserved unchanged. The service layer accepts `QuantityDTO` inputs that cover every unit and operation previously tested, and delegates to the identical `Quantity<TUnit>` arithmetic engine. Legacy `FeetService`, `InchesService`, and `LengthService` classes are retained inside the service file for `FeetTests.cs`, `InchesTest.cs`, and `ExtendedUnitSupport.cs` compatibility.

---

## Conversion Reference

### Length — Base unit: FEET

| From | To FEET |
|---|---|
| 1 FOOT | 1.0 |
| 1 INCH | 0.0833 |
| 1 YARD | 3.0 |
| 1 CENTIMETER | 0.0328 |

### Weight — Base unit: KILOGRAM

| From | To KILOGRAM |
|---|---|
| 1 KILOGRAM | 1.0 |
| 1 GRAM | 0.001 |
| 1 POUND | 0.453592 |

### Volume — Base unit: LITRE

| From | To LITRE |
|---|---|
| 1 LITRE | 1.0 |
| 1 MILLILITRE | 0.001 |
| 1 GALLON (US) | 3.78541 |

### Temperature — Base unit: CELSIUS

| Conversion | Formula |
|---|---|
| CELSIUS → FAHRENHEIT | `(C × 9/5) + 32` |
| FAHRENHEIT → CELSIUS | `(F − 32) × 5/9` |
| CELSIUS → KELVIN | `C + 273.15` |
| KELVIN → CELSIUS | `K − 273.15` |

---

## Console Menu

```
Quantity Measurement Application
---------------------------------
1. Length Operations
2. Weight Operations
3. Volume Operations
4. Temperature Operations
5. Operation History
0. Exit
```

Each category option leads to a sub-menu:

```
LENGTH Operations
1. Compare
2. Convert
3. Add
4. Subtract
5. Divide
```

The **Operation History** option (5) displays all recorded `QuantityMeasurementEntity` entries — including errors — retrieved from `IQuantityMeasurementRepository`.

---

## Test Summary

| Test File | UC | Tests |
|---|---|---|
| `FeetTests.cs` | UC1 | 17 |
| `InchesTest.cs` | UC2 | 11 |
| `QuantityTests.cs` | UC3 | 10 |
| `ExtendedUnitSupport.cs` | UC4 | 17 |
| `LengthConversionTests.cs` | UC5 | 12 |
| `LengthAdditionTests.cs` | UC6 | 13 |
| `LengthAdditionTargetUnitTests.cs` | UC7 | 14 |
| `RefactoredDesignTests.cs` | UC8 | 25 |
| `WeightTests.cs` | UC9 | 28 |
| `GenericQuantityTests.cs` | UC10 | 35 |
| `VolumeTests.cs` | UC11 | 50 |
| `SubtractionAndDivisionTests.cs` | UC12 | 40 |
| `CentralizedArithmeticLogicTests.cs` | UC13 | 40 |
| `TemperatureMeasurementTests.cs` | UC14 | 41 |
| `NTierArchitectureTests.cs` | UC15 | 40 |
| **Total** | | **393** |

All tests run in parallel via `MSTestSettings.cs` (`ExecutionScope.MethodLevel`).

### UC15 Test Coverage

`NTierArchitectureTests.cs` exercises the full N-Tier stack across five categories:

**Entity tests** — construction of single-operand, binary, and error `QuantityMeasurementEntity` records; immutability verification; `ToString()` formatting for both success and error cases.

**Service tests** — same-unit and cross-unit comparison; unit conversion; addition, subtraction, and division; cross-category rejection; unsupported temperature arithmetic; division by zero; null input rejection.

**Controller tests** — integration between controller and service for all five operations; error display formatting; console output format (timestamp brackets, `=>` separator).

**Layer separation tests** — service independence (no controller needed); controller independence (mock service injection); data flow from controller to service and back.

**Backward compatibility & scalability** — single test runs all UC1–UC14 scenarios via the UC15 service layer, confirming zero behavioral regression; all four measurement categories exercised; all thirteen unit implementations verified; operation type tracking in repository history; loose coupling via interface substitution.

---

## Build and Run

**Prerequisites:** .NET 10 SDK

```bash
# Clone the repository
git clone <repository-url>
cd QuantityMeasurementApp

# Build
dotnet build

# Run the application
dotnet run --project QuantityMeasurementApp/QuantityMeasurementApp

# Run all tests
dotnet test
```

---

## Design Principles

| Principle | How it is applied |
|---|---|
| Single Responsibility | Each layer has one job: App handles I/O, BLL handles logic, Repository handles storage, Model holds data |
| Open / Closed | New measurement categories plug into `Quantity<TUnit>` and `IQuantityMeasurementService` without modifying existing code |
| Dependency Inversion | Controller depends on `IQuantityMeasurementService`; service depends on `IQuantityMeasurementRepository`; neither depends on concrete implementations |
| Interface Segregation | `ITemperatureService` exposes only equality and conversion — no arithmetic leaks through; `IQuantityMeasurementApp` exposes only `Run()` |
| DRY | UC13 centralised all arithmetic into `PerformBaseArithmetic`; UC15 centralises all operations into one service and one controller |
| Immutability | Every operation (`Add`, `Subtract`, `ConvertTo`) returns a new object; `QuantityMeasurementEntity` has no setters |
| Layer Separation | Repository stays thin — it records entities without any domain logic; service delegates storage after every operation |
| Default Interface Methods | UC14 uses C# default interface methods on `IMeasurable` to make arithmetic opt-out for temperature without breaking existing units |
| Functional Interfaces | Lambda expressions (`Func<double, double>`) implement temperature conversion formulas; the `SupportsArithmetic` delegate mirrors the `DoubleBinaryOperator` pattern |
| Facade Pattern | `QuantityMeasurementController` hides the service and repository complexity behind simple `PerformXxx` methods, ready to map to REST endpoints |
| Factory / Strategy | `ResolveUnit` in the service acts as a factory, mapping `QuantityDTO` string fields to the correct `IMeasurable` struct at runtime |
