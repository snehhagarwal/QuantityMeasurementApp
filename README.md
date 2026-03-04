# Quantity Measurement Application

A progressive .NET 10 console application for performing measurement equality, conversion, and arithmetic across Length, Weight, and Volume categories. The system grows incrementally through twelve use cases — from a simple feet comparison to a fully generic, extensible arithmetic engine built on the `IMeasurable` interface and `Quantity<TUnit>`.

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
| Use Cases | UC1 – UC12 |
| Measurement Categories | Length, Weight, Volume |
| Supported Units | 10 across 3 categories |
| Operations | Equality, Conversion, Add, Subtract, Divide |
| Total Tests | 329 |
| Test Framework | MSTest 4.x |

---

## Architecture

The application follows a strict five-layer architecture. Each layer has a single responsibility and depends only on the layer below it.

```
┌──────────────────────────────────────────────────────────┐
│  PresentationLayer                                       │
│  FeetPresentation  InchesPresentation  LengthPresentation│
│  LengthPresentationUC4–UC7  WeightPresentationUC9        │
│  QuantityPresentation  VolumePresentation                │
│  SubtractionAndDivisionPresentationUC12                  │
├──────────────────────────────────────────────────────────┤
│  BusinessLogicLayer                                      │
│  FeetService  InchesService  LengthService               │
│  WeightService  VolumeService                            │
├──────────────────────────────────────────────────────────┤
│  DataAccessLayer                                         │
│  FeetRepository  InchesRepository  LengthRepository      │
│  WeightRepository  VolumeRepository                      │
├──────────────────────────────────────────────────────────┤
│  Entities                                                │
│  Feet  Inches  Length  Weight  Quantity<TUnit>           │
│  LengthUnit  WeightUnit  VolumeUnit  (+ Extensions)      │
│  LengthUnitMeasurable  WeightUnitMeasurable              │
│  VolumeUnitMeasurable                                    │
├──────────────────────────────────────────────────────────┤
│  Interface                                               │
│  IMeasurable  IFeetService  IInchesService               │
│  ILengthService  IWeightService  IVolumeService          │
└──────────────────────────────────────────────────────────┘
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
│   │   └── WeightUnitMeasurable.cs
│   │
│   ├── Interface/
│   │   ├── IMeasurable.cs
│   │   ├── IFeetService.cs
│   │   ├── IInchesService.cs
│   │   ├── ILengthService.cs
│   │   ├── IWeightService.cs
│   │   └── IVolumeService.cs
│   │
│   ├── DataAccessLayer/
│   │   ├── FeetRepository.cs
│   │   ├── InchesRepository.cs
│   │   ├── LengthRepository.cs
│   │   ├── WeightRepository.cs
│   │   └── VolumeRepository.cs
│   │
│   ├── BusinessLogicLayer/
│   │   ├── FeetService.cs
│   │   ├── InchesService.cs
│   │   ├── LengthService.cs
│   │   ├── WeightService.cs
│   │   └── VolumeService.cs
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
│   │   └── SubtractionAndDivisionPresentationUC12.cs
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
var feet   = new Length(1.0, LengthUnit.FEET);
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
var feet    = new Length(1.0, LengthUnit.FEET);
var inches  = feet.ConvertTo(LengthUnit.INCHES);  // Length(12.0, INCHES)
var yards   = feet.ConvertTo(LengthUnit.YARDS);   // Length(0.33, YARDS)
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
double feet  = LengthUnit.INCHES.ConvertToBaseUnit(12.0);  // 1.0
double inch  = LengthUnit.FEET.ConvertFromBaseUnit(1.0);   // 12.0
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

bool equal = feet.Equals(inches);                                 // true
var  sum   = feet.Add(inches);                                    // Quantity(2.0, FEET)
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

var diff        = a.Subtract(b);                                                // Quantity(9.5, FEET)
var diffInInches = a.Subtract(b, new LengthUnitMeasurable(LengthUnit.INCHES)); // Quantity(114.0, INCHES)
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

Negative results from subtraction are valid. Immutability is preserved — originals are never modified. The `IVolumeService` interface was extended with matching `Subtract` and `Divide` signatures, and `VolumeRepository` and `VolumeService` were updated to delegate accordingly.

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
0.  Exit
```

Each menu option maps to exactly one `*Presentation` class. All presentation classes depend on a service interface, making this a straightforward controller registration list if the application migrates to ASP.NET.

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
| **Total** | | **272** |

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
| Dependency Inversion | Each service class depends on its interface (`IFeetService`, `IVolumeService`, etc.) |
| Interface Segregation | `IMeasurable` defines only the conversion contract — no arithmetic leaks in |
| DRY | `Quantity<TUnit>` centralises equality, conversion, and arithmetic for all categories |
| Immutability | Every operation (`Add`, `Subtract`, `ConvertTo`) returns a new object; originals are unchanged |
| Layer Separation | DAL stays thin — it delegates all domain logic to entity methods and never recomputes |
