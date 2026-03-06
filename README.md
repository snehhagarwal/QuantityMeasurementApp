# Quantity Measurement Application

A progressive .NET 10 console application for performing measurement equality, conversion, and arithmetic across Length, Weight, Volume, and Temperature categories. The system grows incrementally through fourteen use cases — from a simple feet comparison to a fully generic, extensible arithmetic engine built on the `IMeasurable` interface and `Quantity<TUnit>`, culminating in non-linear temperature support with selective arithmetic enforcement.

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
| Use Cases | UC1 – UC14 |
| Measurement Categories | Length, Weight, Volume, Temperature |
| Supported Units | 13 across 4 categories |
| Operations | Equality, Conversion, Add, Subtract, Divide |
| Total Tests | 353 |
| Test Framework | MSTest 4.x |

---

## Architecture

The application follows a strict five-layer architecture. Each layer has a single responsibility and depends only on the layer below it.

```
┌──────────────────────────────────────────────────────────────┐
│  PresentationLayer                                           │
│  FeetPresentation  InchesPresentation  LengthPresentation    │
│  LengthPresentationUC4–UC7  WeightPresentationUC9            │
│  QuantityPresentation  VolumePresentation                    │
│  SubtractionAndDivisionPresentationUC12                      │
│  TemperaturePresentation                                     │
├──────────────────────────────────────────────────────────────┤
│  BusinessLogicLayer                                          │
│  FeetService  InchesService  LengthService                   │
│  WeightService  VolumeService  TemperatureService            │
├──────────────────────────────────────────────────────────────┤
│  DataAccessLayer                                             │
│  FeetRepository  InchesRepository  LengthRepository          │
│  WeightRepository  VolumeRepository  TemperatureRepository   │
├──────────────────────────────────────────────────────────────┤
│  Entities                                                    │
│  Feet  Inches  Length  Weight  Quantity<TUnit>               │
│  LengthUnit  WeightUnit  VolumeUnit  TemperatureUnit         │
│  *UnitExtensions (×4)   *UnitMeasurable (×4)                │
├──────────────────────────────────────────────────────────────┤
│  Interface                                                   │
│  IMeasurable  IFeetService  IInchesService  ILengthService   │
│  IWeightService  IVolumeService  ITemperatureService         │
└──────────────────────────────────────────────────────────────┘
```

**Dependency direction:** Presentation → BLL → DAL → Entities → Interface

Each `*Service` class accepts its repository via a parameterless constructor (default) or a DI constructor — making the full stack ready for ASP.NET Controller migration without changes.

---

## Project Structure

```
QuantityMeasurementApp/
│
├── QuantityMeasurementApp/
│   ├── Entities/
│   │   ├── Feet.cs
│   │   ├── Inches.cs
│   │   ├── Length.cs
│   │   ├── LengthUnit.cs
│   │   ├── LengthUnitExtensions.cs
│   │   ├── LengthUnitMeasurable.cs
│   │   ├── Quantity.cs
│   │   ├── VolumeUnit.cs
│   │   ├── VolumeUnitExtensions.cs
│   │   ├── VolumeUnitMeasurable.cs
│   │   ├── Weight.cs
│   │   ├── WeightUnit.cs
│   │   ├── WeightUnitExtensions.cs
│   │   ├── WeightUnitMeasurable.cs
│   │   ├── TemperatureUnit.cs
│   │   ├── TemperatureUnitExtensions.cs
│   │   └── TemperatureUnitMeasurable.cs
│   │
│   ├── Interface/
│   │   ├── IMeasurable.cs
│   │   ├── IFeetService.cs
│   │   ├── IInchesService.cs
│   │   ├── ILengthService.cs
│   │   ├── IWeightService.cs
│   │   ├── IVolumeService.cs
│   │   └── ITemperatureService.cs
│   │
│   ├── DataAccessLayer/
│   │   ├── FeetRepository.cs
│   │   ├── InchesRepository.cs
│   │   ├── LengthRepository.cs
│   │   ├── WeightRepository.cs
│   │   ├── VolumeRepository.cs
│   │   └── TemperatureRepository.cs
│   │
│   ├── BusinessLogicLayer/
│   │   ├── FeetService.cs
│   │   ├── InchesService.cs
│   │   ├── LengthService.cs
│   │   ├── WeightService.cs
│   │   ├── VolumeService.cs
│   │   └── TemperatureService.cs
│   │
│   ├── PresentationLayer/
│   │   ├── FeetPresentation.cs
│   │   ├── InchesPresentation.cs
│   │   ├── LengthPresentation.cs
│   │   ├── LengthPresentationUC4.cs
│   │   ├── LengthPresentationUC5.cs
│   │   ├── LengthPresentationUC6.cs
│   │   ├── LengthPresentationUC7.cs
│   │   ├── WeightPresentationUC9.cs
│   │   ├── QuantityPresentation.cs
│   │   ├── VolumePresentation.cs
│   │   ├── SubtractionAndDivisionPresentationUC12.cs
│   │   └── TemperaturePresentation.cs
│   │
│   ├── Program.cs
│   └── QuantityMeasurementApp.csproj
│
└── QuantityMeasurementApp.Tests/
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
    └── MSTestSettings.cs
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

Cross-category operations are prevented at compile time by the generic type parameter — `Quantity<LengthUnitMeasurable>` and `Quantity<WeightUnitMeasurable>` are distinct types.

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

Negative results from subtraction are valid. Immutability is preserved — originals are never modified.

---

### UC13 — Centralized Arithmetic Logic (DRY Refactor)

A pure internal refactor of `Quantity<TUnit>` — no new features, no new units, no API changes. The goal is to eliminate code duplication across `Add`, `Subtract`, and `Divide` by extracting shared arithmetic logic into centralized helpers.

**Before UC13:** Each arithmetic method (`Add`, `Subtract`, `Divide`) repeated the same base-unit conversion, validation, and result-building steps independently.

**After UC13:** All three methods delegate to a single `PerformBaseArithmetic` helper via the `ArithmeticOperation` enum, mirroring the `DoubleBinaryOperator` functional interface pattern.

```csharp
// ArithmeticOperation enum drives the operation selector
private enum ArithmeticOperation { Add, Subtract, Divide }

// All three public methods delegate to this single helper
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

Four private helpers introduced: `ValidateArithmeticOperands`, `PerformBaseArithmetic`, `BuildResult`, and the `ArithmeticOperation` enum itself. All 262 existing tests pass without modification — the refactor is fully transparent to callers.

---

### UC14 — Temperature Measurement

Introduces `TemperatureUnit` (CELSIUS, FAHRENHEIT, KELVIN) as the fourth measurement category. Temperature is physically meaningful for equality and conversion, but **arithmetic on temperature is undefined** — adding or dividing absolute temperatures produces physically meaningless results.

**Key design decisions:**

**1. Selective arithmetic via IMeasurable default methods.**
Rather than forcing `TemperatureUnitMeasurable` to implement dummy arithmetic, `IMeasurable` was evolved with two optional default methods:

```csharp
public interface IMeasurable
{
    // ... existing 4 methods unchanged ...

    // UC14: default methods — existing units inherit these unchanged
    bool SupportsArithmeticOps()               => true;
    void ValidateOperationSupport(string op)   { }  // no-op default
}
```

`TemperatureUnitMeasurable` overrides both to block arithmetic:

```csharp
public bool SupportsArithmeticOps() => false;  // lambda: () => false

public void ValidateOperationSupport(string operation)
    => throw new NotSupportedException($"Temperature does not support {operation}.");
```

All existing units (`LengthUnitMeasurable`, `WeightUnitMeasurable`, `VolumeUnitMeasurable`) inherit the default `true` — **zero changes required**.

**2. Non-linear conversion via lambda expressions.**
Temperature conversions use offset-based formulas, not simple multiplication factors. `TemperatureUnitExtensions` uses `Func<double, double>` lambdas for each path:

```csharp
private static readonly Func<double, double> CelsiusToFahrenheit = c => (c * 9.0 / 5.0) + 32.0;
private static readonly Func<double, double> FahrenheitToCelsius = f => (f - 32.0) * 5.0 / 9.0;
private static readonly Func<double, double> CelsiusToKelvin     = c => c + 273.15;
private static readonly Func<double, double> KelvinToCelsius     = k => k - 273.15;
```

**3. Epsilon-tolerant equality.**
`Quantity.Equals` uses `Math.Abs(ToBaseUnit() - other.ToBaseUnit()) < 1e-9` to absorb floating-point round-trip errors in temperature conversions.

**4. Arithmetic blocked at the entry point.**
`Quantity<TUnit>.PerformBaseArithmetic` calls `this.Unit.ValidateOperationSupport(operation)` before touching any numbers — temperature throws immediately, all other units proceed normally.

```csharp
// Equality and conversion — fully supported
new Quantity<TemperatureUnitMeasurable>(0.0,   CELSIUS).Equals(
new Quantity<TemperatureUnitMeasurable>(32.0,  FAHRENHEIT))     // true
new Quantity<TemperatureUnitMeasurable>(100.0, CELSIUS).ConvertTo(FAHRENHEIT)  // 212.0 FAHRENHEIT

// Arithmetic — throws NotSupportedException
new Quantity<TemperatureUnitMeasurable>(100.0, CELSIUS).Add(
new Quantity<TemperatureUnitMeasurable>(50.0,  CELSIUS))        // NotSupportedException: Temperature does not support Add.

// Cross-category — always false
new Quantity<TemperatureUnitMeasurable>(100.0, CELSIUS).Equals(
new Quantity<LengthUnitMeasurable>(100.0,      FEET))           // false
```

**Temperature conversion formulas:**

| From | To | Formula |
|---|---|---|
| CELSIUS | FAHRENHEIT | `(C × 9/5) + 32` |
| FAHRENHEIT | CELSIUS | `(F − 32) × 5/9` |
| CELSIUS | KELVIN | `C + 273.15` |
| KELVIN | CELSIUS | `K − 273.15` |
| FAHRENHEIT | KELVIN | via CELSIUS intermediate |
| KELVIN | FAHRENHEIT | via CELSIUS intermediate |

**Notable conversion points:**

| Value | Equivalent |
|---|---|
| 0°C | 32°F = 273.15 K |
| 100°C | 212°F = 373.15 K |
| −40°C | −40°F (scales intersect here) |
| 0 K | −273.15°C = −459.67°F (absolute zero) |

The service interface `ITemperatureService` exposes only `AreEqual` and `ConvertTo` — no arithmetic methods — enforcing the constraint at the API boundary as well.

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
----------------------------------
1.  Feet Equality
2.  Feet and Inches Equality
3.  Generic Length Equality
4.  Extended Units Equality
5.  Unit-to-Unit Conversion
6.  Addition of Lengths
7.  Addition with Target Unit
8.  Weight Measurement
9.  Generic Quantity Measurement
10. Volume Measurement
11. Subtraction and Division
12. Temperature Measurement
0.  Exit
```

Each menu option maps to exactly one `*Presentation` class. All presentation classes depend on a service interface, making this a straightforward controller registration list if the application migrates to ASP.NET.

**Temperature Measurement sub-menu (option 12):**

```
UC14: Temperature Measurement (Celsius, Fahrenheit, Kelvin)

1. Equality Comparison
2. Unit Conversion
3. Unsupported Arithmetic (Add / Subtract / Divide)
4. Cross-Category Prevention
```

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
| `CentralizedArithmeticLogicTests.cs` | UC13 | 24 |
| `TemperatureMeasurementTests.cs` | UC14 | 41 |
| **Total** | | **353** |

All tests run in parallel via `MSTestSettings.cs` (`ExecutionScope.MethodLevel`).

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
dotnet run --project QuantityMeasurementApp

# Run all tests
dotnet test
```

---

## Design Principles

| Principle | How it is applied |
|---|---|
| Single Responsibility | Each unit enum owns its own conversion logic via extension methods |
| Open / Closed | New measurement categories plug into `Quantity<TUnit>` without modifying it |
| Dependency Inversion | Each service class depends on its interface (`IFeetService`, `IVolumeService`, `ITemperatureService`, etc.) |
| Interface Segregation | `ITemperatureService` exposes only equality and conversion — no arithmetic methods leak through |
| DRY | UC13 centralised all arithmetic into `PerformBaseArithmetic`; `Quantity<TUnit>` handles all categories |
| Immutability | Every operation (`Add`, `Subtract`, `ConvertTo`) returns a new object; originals are unchanged |
| Layer Separation | DAL stays thin — it delegates all domain logic to entity methods and never recomputes |
| Default Interface Methods | UC14 uses C# default interface methods on `IMeasurable` to make arithmetic opt-out for temperature without breaking existing units |
| Functional Interfaces | Lambda expressions (`Func<double, double>`) implement temperature conversion formulas; the `SupportsArithmetic` delegate mirrors the `DoubleBinaryOperator` pattern |
