# Quantity Measurement Application

A progressive **.NET 8 ASP.NET Core Web API** for performing measurement equality, conversion, and arithmetic across Length, Weight, Volume, and Temperature categories. The system grows incrementally through **eighteen use cases** — from a simple feet comparison to a fully secured REST API with JWT authentication, AES encryption, Redis caching, EF Core persistence, and a complete HTML/CSS/JS frontend.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Use Cases](#use-cases)
  - [UC1 — Feet Equality](#uc1--feet-equality)
  - [UC2 — Inches Equality](#uc2--inches-equality)
  - [UC3 — Generic Length Equality](#uc3--generic-length-equality)
  - [UC4 — Extended Unit Support](#uc4--extended-unit-support)
  - [UC5 — Unit Conversion](#uc5--unit-conversion)
  - [UC6 — Addition of Lengths](#uc6--addition-of-lengths)
  - [UC7 — Addition with Explicit Target Unit](#uc7--addition-with-explicit-target-unit)
  - [UC8 — Length Unit Refactoring](#uc8--length-unit-refactoring)
  - [UC9 — Weight Measurement](#uc9--weight-measurement)
  - [UC10 — Generic Quantity Class](#uc10--generic-quantity-class)
  - [UC11 — Volume Measurement](#uc11--volume-measurement)
  - [UC12 — Subtraction and Division](#uc12--subtraction-and-division)
  - [UC13 — Centralized Arithmetic Logic](#uc13--centralized-arithmetic-logic)
  - [UC14 — Temperature Measurement](#uc14--temperature-measurement)
  - [UC15 — N-Tier Architecture](#uc15--n-tier-architecture)
  - [UC16 — Database Repository](#uc16--database-repository)
  - [UC17 — REST API](#uc17--rest-api)
  - [UC18 — Secure Authentication](#uc18--secure-authentication)
- [Conversion Reference](#conversion-reference)
- [REST API Reference](#rest-api-reference)
- [Test Summary](#test-summary)
- [Build and Run](#build-and-run)
- [Design Principles](#design-principles)

---

## Overview

| Metric | Value |
|---|---|
| Framework | .NET 8 / ASP.NET Core Web API |
| Use Cases | UC1 – UC18 |
| Measurement Categories | Length, Weight, Volume, Temperature |
| Supported Units | 13 across 4 categories |
| Operations | Equality, Conversion, Add, Subtract, Divide |
| Total Tests | 473+ |
| Test Framework | MSTest 4.x |
| Database | SQL Server (EF Core) / In-Memory |
| Cache | Redis + In-Memory fallback |
| Auth | JWT Bearer + AES-256 Encryption |
| Frontend | HTML5 / CSS3 / ES9 JavaScript (UC19) |

---

## Architecture

UC15 established the four-project N-Tier structure. UC16–UC18 extended it with database persistence, REST API, and security layers. Dependency direction flows strictly downward.

```
┌──────────────────────────────────────────────────────────────────────┐
│  QuantityMeasurementApi  (ASP.NET Core Web API — Presentation)       │
│  QuantityMeasurementController   UserController                      │
│  GlobalExceptionHandlingMiddleware   ApiSecurityExtensions           │
│  wwwroot/  ← UC19 HTML/CSS/JS Frontend (index.html)                 │
├──────────────────────────────────────────────────────────────────────┤
│  QuantityMeasurementBusinessLayer                                    │
│  IQuantityMeasurementService   QuantityMeasurementServiceImpl        │
│  IUserService   UserService                                          │
│  IJwtTokenService   JwtTokenService                                  │
│  IAesEncryptionService   AesEncryptionService                        │
│  IMeasurable   Quantity<TUnit>   *UnitMeasurable (×4)                │
│  LengthUnitExtensions   WeightUnitExtensions                         │
│  VolumeUnitExtensions   TemperatureUnitExtensions                    │
│  QuantityMeasurementException                                        │
├──────────────────────────────────────────────────────────────────────┤
│  QuantityMeasurementRepository                                       │
│  IQuantityMeasurementEntityRepository                                │
│  CacheRepository (In-Memory + JSON file)                             │
│  RedisRepository (Redis PRIMARY + SQL Server dual-write)             │
│  EfQuantityRepository (EF Core — SQL Server / InMemory)             │
│  EfUserRepository (EF Core — Users table)                           │
│  ICacheService   RedisCacheService   MemoryCacheService              │
│  DatabaseException                                                   │
├──────────────────────────────────────────────────────────────────────┤
│  QuantityMeasurementModel                                            │
│  QuantityDTO   QuantityMeasurementEntity   QuantityInputDto          │
│  QuantityOperandDto   QuantityMeasurementDto                         │
│  RegisterDto   LoginDto   AuthResponseDto   User (EF entity)         │
│  LengthUnit   WeightUnit   VolumeUnit   TemperatureUnit              │
│  Feet   Inches   Length   Weight                                     │
└──────────────────────────────────────────────────────────────────────┘
```

---

## Project Structure

```
QuantityMeasurementApp/
│
├── QuantityMeasurementApi/                  ← ASP.NET Core Web API (entry point)
│   ├── Controllers/
│   │   ├── QuantityMeasurementController.cs
│   │   └── UserController.cs
│   ├── Middleware/
│   │   ├── ApiSecurityExtensions.cs
│   │   └── GlobalExceptionHandlingMiddleware.cs
│   ├── wwwroot/                             ← UC19 Frontend (static files)
│   │   ├── index.html
│   │   ├── style.css
│   │   ├── app.js
│   │   └── users.json
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── appsettings.Production.json
│
├── QuantityMeasurementBusinessLayer/        ← Business logic
│   ├── Interface/
│   │   ├── IMeasurable.cs
│   │   ├── IQuantityMeasurementService.cs
│   │   ├── IUserService.cs
│   │   ├── IJwtTokenService.cs
│   │   └── IAesEncryptionService.cs
│   ├── Service/
│   │   ├── Quantity.cs
│   │   ├── QuantityMeasurementServiceImpl.cs
│   │   ├── UserService.cs
│   │   ├── JwtTokenService.cs
│   │   ├── AesEncryptionService.cs
│   │   └── UnitMeasurables.cs
│   ├── Unit/
│   │   ├── LengthUnitExtensions.cs
│   │   ├── WeightUnitExtensions.cs
│   │   ├── VolumeUnitExtensions.cs
│   │   └── TemperatureUnitExtensions.cs
│   └── Exceptions/
│       └── QuantityMeasurementException.cs
│
├── QuantityMeasurementRepository/           ← Persistence layer
│   ├── Interface/
│   │   ├── IQuantityMeasurementEntityRepository.cs
│   │   └── IQuantityMeasurementRepository.cs
│   ├── Repository/
│   │   ├── CacheRepository.cs
│   │   ├── RedisRepository.cs
│   │   └── EfQuantityRepository.cs (UserRepository)
│   ├── Service/
│   │   ├── RedisCacheService.cs
│   │   └── MemoryCacheService.cs
│   └── Exception/
│       └── DatabaseException.cs
│
├── QuantityMeasurementModel/                ← Pure data model
│   ├── Dto/
│   │   ├── QuantityDTO.cs
│   │   ├── QuantityMeasurementEntity.cs
│   │   ├── QuantityInputDto.cs
│   │   ├── QuantityOperandDto.cs
│   │   ├── QuantityMeasurementDto.cs
│   │   └── Auth/
│   │       ├── RegisterDto.cs
│   │       ├── LoginDto.cs
│   │       └── AuthResponseDto.cs
│   ├── Context/
│   │   └── QuantityMeasurementDbContext.cs
│   └── Entities/
│       ├── User.cs
│       ├── Feet.cs
│       ├── Inches.cs
│       ├── Length.cs
│       ├── Weight.cs
│       ├── LengthUnit.cs
│       ├── WeightUnit.cs
│       ├── VolumeUnit.cs
│       └── TemperatureUnit.cs
│
├── QuantityMeasurementApp.Tests/            ← UC1–UC16 tests (MSTest)
│   ├── FeetTests.cs
│   ├── InchesTest.cs
│   ├── QuantityTests.cs
│   ├── ExtendedUnitSupport.cs
│   ├── LengthConversionTests.cs
│   ├── LengthAdditionTests.cs
│   ├── LengthAdditionTargetUnitTests.cs
│   ├── RefactoredDesignTests.cs
│   ├── WeightTests.cs
│   ├── GenericQuantityTests.cs
│   ├── VolumeTests.cs
│   ├── SubtractionAndDivisionTests.cs
│   ├── CentralizedArithmeticLogicTests.cs
│   ├── TemperatureMeasurementTests.cs
│   ├── NTierArchitectureTests.cs
│   ├── DatabaseRepositoryTests.cs
│   └── MSTestSettings.cs
│
└── QuantityMeasurementApi.Tests/            ← UC17–UC18 tests (MSTest + WebApplicationFactory)
    ├── Controllers/
    │   └── QuantityMeasurementControllerTest.cs
    └── Integration/
        └── QuantityMeasurementApplicationTests.cs
```

---

## Use Cases

---

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
var inches = feet.ConvertTo(LengthUnit.INCHES); // Length(12.0, INCHES)
var yards  = feet.ConvertTo(LengthUnit.YARDS);  // Length(0.33, YARDS)
```

---

### UC6 — Addition of Lengths

Adds `Add()` to the `Length` class. Result is expressed in the first operand's unit.

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

Overloads `Add()` to accept a `targetUnit` parameter.

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

Moves conversion responsibility from the `Length` class into `LengthUnit` itself via C# extension methods. Each unit now owns its own conversion logic — a Single Responsibility improvement.

```csharp
double feet = LengthUnit.INCHES.ConvertToBaseUnit(12.0);   // 1.0
double inch = LengthUnit.FEET.ConvertFromBaseUnit(1.0);    // 12.0
```

`LengthUnitMeasurable` wraps the enum as an `IMeasurable` struct, bridging it to the generic `Quantity<TUnit>` class introduced in UC10.

---

### UC9 — Weight Measurement

Introduces the `Weight` entity with `WeightUnit` enum. Base unit is KILOGRAM.

```csharp
var kg   = new Weight(1.0,    WeightUnit.KILOGRAM);
var gram = new Weight(1000.0, WeightUnit.GRAM);

bool equal = kg.Equals(gram); // true
var  sum   = kg.Add(gram);    // Weight(2.0, KILOGRAM)
```

| Unit | Conversion to KILOGRAM |
|---|---|
| KILOGRAM | × 1.0 |
| GRAM | × 0.001 |
| POUND | × 0.453592 |

---

### UC10 — Generic Quantity Class

Introduces `IMeasurable` and `Quantity<TUnit>` — a single generic class replacing all per-category classes.

```csharp
public interface IMeasurable
{
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();
    bool SupportsArithmeticOps()             => true;   // default
    void ValidateOperationSupport(string op) { }        // default
}
```

```csharp
var feet   = new Quantity<LengthUnitMeasurable>(1.0,  new LengthUnitMeasurable(LengthUnit.FEET));
var inches = new Quantity<LengthUnitMeasurable>(12.0, new LengthUnitMeasurable(LengthUnit.INCHES));

bool equal = feet.Equals(inches);                                           // true
var  sum   = feet.Add(inches);                                              // Quantity(2.0, FEET)
var  conv  = feet.ConvertTo(new LengthUnitMeasurable(LengthUnit.INCHES));  // Quantity(12.0, INCHES)
```

Cross-category operations are prevented at compile time by the generic type parameter.

---

### UC11 — Volume Measurement

Adds `VolumeUnit`, `VolumeUnitExtensions`, and `VolumeUnitMeasurable`. Plugs directly into `Quantity<TUnit>` — zero changes to the generic infrastructure. Base unit is LITRE.

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

Adds two new arithmetic operations to `Quantity<TUnit>`. Works across Length, Weight, and Volume automatically.

```csharp
var a = new Quantity<LengthUnitMeasurable>(10.0, new LengthUnitMeasurable(LengthUnit.FEET));
var b = new Quantity<LengthUnitMeasurable>(6.0,  new LengthUnitMeasurable(LengthUnit.INCHES));

var diff  = a.Subtract(b);  // Quantity(9.5, FEET)
double ratio = a.Divide(b); // 20.0  (dimensionless scalar)
```

| Operation | Result |
|---|---|
| 10 ft − 6 in | 9.5 FEET |
| 5 L − 500 mL | 4.5 LITRE |
| 10 ft ÷ 2 ft | 5.0 (scalar) |
| 1 L ÷ 500 mL | 2.0 (scalar) |
| anything ÷ 0 | `ArithmeticException` |

---

### UC13 — Centralized Arithmetic Logic

A pure DRY refactor of `Quantity<TUnit>`. Eliminates duplication across `Add`, `Subtract`, and `Divide` by extracting shared logic into a single helper. All existing tests pass without modification.

```csharp
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

---

### UC14 — Temperature Measurement

Introduces `TemperatureUnit` (CELSIUS, FAHRENHEIT, KELVIN). Temperature supports equality and conversion but **not arithmetic** — adding temperatures is physically meaningless.

```csharp
// Equality and conversion — fully supported
new Quantity<TemperatureUnitMeasurable>(0.0,  CELSIUS).Equals(
new Quantity<TemperatureUnitMeasurable>(32.0, FAHRENHEIT))     // true

// Arithmetic — throws NotSupportedException
new Quantity<TemperatureUnitMeasurable>(100.0, CELSIUS).Add(
new Quantity<TemperatureUnitMeasurable>(50.0,  CELSIUS))       // NotSupportedException
```

Arithmetic is blocked via `IMeasurable` default methods — `TemperatureUnitMeasurable` overrides `SupportsArithmeticOps()` to return `false`. All existing units inherit the default `true` with zero changes required.

Non-linear conversion uses lambda expressions:

```csharp
private static readonly Func<double, double> CelsiusToFahrenheit = c => (c * 9.0 / 5.0) + 32.0;
private static readonly Func<double, double> FahrenheitToCelsius = f => (f - 32.0) * 5.0 / 9.0;
private static readonly Func<double, double> CelsiusToKelvin     = c => c + 273.15;
private static readonly Func<double, double> KelvinToCelsius     = k => k - 273.15;
```

| From | To | Formula |
|---|---|---|
| CELSIUS | FAHRENHEIT | `(C × 9/5) + 32` |
| FAHRENHEIT | CELSIUS | `(F − 32) × 5/9` |
| CELSIUS | KELVIN | `C + 273.15` |
| KELVIN | CELSIUS | `K − 273.15` |

---

### UC15 — N-Tier Architecture

Restructures the application into a clean four-project N-Tier solution. Six separate service interfaces are replaced by a single `IQuantityMeasurementService` with five operations.

```csharp
public interface IQuantityMeasurementService
{
    Task<QuantityMeasurementDto> CompareAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken ct);
    Task<QuantityMeasurementDto> ConvertAsync(QuantityOperandDto quantity, QuantityOperandDto target, CancellationToken ct);
    Task<QuantityMeasurementDto> AddAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken ct);
    Task<QuantityMeasurementDto> SubtractAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken ct);
    Task<QuantityMeasurementDto> DivideAsync(QuantityOperandDto first, QuantityOperandDto second, CancellationToken ct);
    Task<IReadOnlyList<QuantityMeasurementDto>> GetAllHistoryAsync(CancellationToken ct);
    Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByOperationTypeAsync(string op, CancellationToken ct);
    Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByMeasurementTypeAsync(string type, CancellationToken ct);
    Task<IReadOnlyList<QuantityMeasurementDto>> GetErrorHistoryAsync(CancellationToken ct);
    Task<long> CountByOperationTypeAsync(string op, CancellationToken ct);
}
```

The service maps `QuantityOperandDto` inputs to the appropriate `*UnitMeasurable` via a `ResolveUnit` factory helper:

```csharp
private static IMeasurable ResolveUnit(QuantityOperandDto dto) => dto.MeasurementType switch
{
    "LENGTH"      => new LengthUnitMeasurable(Enum.Parse<LengthUnit>(dto.Unit)),
    "WEIGHT"      => new WeightUnitMeasurable(Enum.Parse<WeightUnit>(dto.Unit)),
    "VOLUME"      => new VolumeUnitMeasurable(Enum.Parse<VolumeUnit>(dto.Unit)),
    "TEMPERATURE" => new TemperatureUnitMeasurable(Enum.Parse<TemperatureUnit>(dto.Unit)),
    _             => throw new QuantityMeasurementException($"Unknown type: {dto.MeasurementType}")
};
```

Every operation is saved as a `QuantityMeasurementEntity` in the repository — including errors — providing a complete audit trail.

---

### UC16 — Database Repository

Replaces the in-memory cache repository with a production-grade persistence layer using **EF Core** and **SQL Server**. Introduces connection pooling, parameterized queries, and a complete suite of database tests.

#### What UC16 adds

- **`QuantityMeasurementDbContext`** — EF Core `DbContext` with `QuantityMeasurements` table mapped to `QuantityMeasurementEntity`
- **`EfQuantityRepository`** — full `IQuantityMeasurementEntityRepository` backed by SQL Server via EF Core
- **`EfUserRepository`** — EF Core repository for `User` entities; replaces in-memory user store
- **Extended `IQuantityMeasurementEntityRepository`** — adds `GetMeasurementsByOperationType()`, `GetMeasurementsByMeasurementType()`, `GetTotalCount()`, `ReleaseResources()`, `GetPoolStatistics()`
- **`DatabaseException`** — custom exception wrapping lower-level database errors
- **InMemory fallback** — `appsettings.json` `UseDatabase: InMemory` lets the API start without SQL Server (used in all tests)
- **SQL injection prevention** — EF Core parameterized queries on all filter methods
- **Thread-safe concurrent access** — tested with 10 threads × 10 saves each

#### Extended repository interface

```csharp
public interface IQuantityMeasurementEntityRepository
{
    void Save(QuantityMeasurementEntity entity);
    IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
    IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType);
    IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType);
    int    GetTotalCount();
    string GetPoolStatistics();
    void   ReleaseResources();
    void   Clear();
}
```

#### Repository mode configuration — `appsettings.json`

```json
// Cache mode (default) — no database required
"RepositoryType": "Cache",
"UseDatabase": "InMemory",

// SQL Server mode
"RepositoryType": "Cache",
"UseDatabase": "SqlServer",
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=QuantityMeasurementDB;Trusted_Connection=True;TrustServerCertificate=True;"
},

// Redis + SQL Server dual-write mode
"RepositoryType": "Redis",
"Redis": { "Enabled": true, "ConnectionString": "localhost:6379" }
```

#### UC16 test coverage — `DatabaseRepositoryTests.cs`

| Category | Tests | What is verified |
|---|---|---|
| Save / Retrieve | 3 | Single save, multiple saves, `GetAllMeasurements()` |
| Query by Operation Type | 3 | Filter by ADD/COMPARE, case-insensitive, no-match returns empty |
| Query by Measurement Type | 1 | Filters by unit keyword in `FirstOperand` |
| Count & Clear | 2 | `GetTotalCount()` increments, `Clear()` resets to zero |
| Error Entity | 1 | `IsError=true` entities stored and retrieved correctly |
| Pool Statistics | 2 | `GetPoolStatistics()` returns non-empty string with repo name |
| Backward Compatibility | 3 | UC1–UC15 data flows through repository unchanged |
| Database-Specific | 8 | Save, retrieve, query, count, deleteAll, full dataset |
| SQL Injection Prevention | 1 | Parameterized queries block injection attempts; original data intact |
| Transaction / Error Handling | 1 | Simulated error leaves count unchanged |
| Data Isolation | 2 | Fresh repo per test — no state leakage between tests |
| Repository Factory | 2 | `CacheRepository` and `InMemoryTestRepository` creation |
| `DatabaseException` | 2 | Custom exception message and inner exception |
| Resource Cleanup | 1 | `ReleaseResources()` does not break subsequent calls |
| Batch Insert | 1 | 50 sequential saves all stored correctly |
| Large Dataset | 1 | 1000 records, `GetAllMeasurements()` completes in < 2 seconds |
| Timestamp Handling | 1 | Saved entity timestamp within expected range |
| Concurrent Access | 1 | 10 threads × 10 saves = 100 total records |
| Package Structure | 1 | All layer types accessible via reflection |

---

### UC17 — REST API

Promotes the application to a fully-featured **HTTP REST API** with controller routing, global exception handling, model validation, health checks, and Swagger/OpenAPI documentation.

#### What UC17 adds

- **`QuantityMeasurementController`** — five POST endpoints + GET history endpoints, all protected with `[Authorize]`
- **`UserController`** — `POST /signup` and `POST /login` — public, no auth required
- **`GlobalExceptionHandlingMiddleware`** — maps exceptions to consistent JSON error responses with timestamp, status, error label, message, and path
- **`ApiSecurityExtensions`** — registers CORS AllowAll policy, Swagger with Bearer auth UI, JWT Bearer validation
- **Request/Response DTOs** — `QuantityInputDto`, `QuantityOperandDto`, `QuantityMeasurementDto`
- **Health check** — `/actuator/health`
- **Swagger** — JSON at `/swagger/v1/swagger.json`, UI at `/swagger`
- **Static file serving** — `UseDefaultFiles()` + `UseStaticFiles()` serve `wwwroot/index.html` automatically at `/`

#### All endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/users/signup` | None | Register new account. Returns JWT. |
| POST | `/api/v1/users/login` | None | Authenticate. Returns JWT. |
| POST | `/api/v1/quantities/compare` | Bearer JWT | Compare two quantities. Returns `true`/`false`. |
| POST | `/api/v1/quantities/convert` | Bearer JWT | Convert value to target unit. |
| POST | `/api/v1/quantities/add` | Bearer JWT | Add two quantities. |
| POST | `/api/v1/quantities/subtract` | Bearer JWT | Subtract second from first. |
| POST | `/api/v1/quantities/divide` | Bearer JWT | Divide first by second (dimensionless ratio). |
| GET | `/api/v1/quantities/history` | Bearer JWT | Full operation history. |
| GET | `/api/v1/quantities/history/operation/{op}` | Bearer JWT | History filtered by operation type. |
| GET | `/api/v1/quantities/history/type/{type}` | Bearer JWT | History filtered by measurement type. |
| GET | `/api/v1/quantities/history/errored` | Bearer JWT | All error records. |
| GET | `/api/v1/quantities/count/{op}` | Bearer JWT | Count operations of given type. |
| GET | `/actuator/health` | None | Health check — returns 200 Healthy. |
| GET | `/swagger/v1/swagger.json` | None | OpenAPI JSON specification. |

#### Request / Response format

```json
// POST /api/v1/quantities/add
{
  "thisQuantityDTO": { "value": 1.0, "unit": "FEET",   "measurementType": "LENGTH" },
  "thatQuantityDTO": { "value": 12.0, "unit": "INCHES", "measurementType": "LENGTH" }
}

// 200 OK
{
  "operation":       "ADD",
  "resultString":    "2 FEET",
  "measurementType": "LENGTH",
  "isError":         false
}
```

#### Global exception mapping

| Exception Type | HTTP Status | Error Label |
|---|---|---|
| `QuantityMeasurementException` | 400 Bad Request | Quantity Measurement Error |
| `UnauthorizedAccessException` | 401 Unauthorized | Unauthorized |
| `ArgumentException` | 409 Conflict | Conflict |
| `ArithmeticException` | 500 Internal Server Error | Internal Server Error |
| Any other exception | 500 Internal Server Error | Internal Server Error |

All error responses include `timestamp`, `status`, `error`, `message`, and `path` fields.

#### UC17 test coverage

| Test Class | Tests | Scope |
|---|---|---|
| `QuantityMeasurementControllerTest` | 14 | Mocked service — unit tests for HTTP status codes and response shapes |
| `QuantityMeasurementApplicationTests` | 40+ | Full integration — real service, EF In-Memory, real HTTP round-trips, no mocking |

---

### UC18 — Secure Authentication

Adds production-grade security to the REST API. Users register and log in to receive **JWT Bearer tokens**. All quantity endpoints require a valid token. Passwords are **AES-256 encrypted** at rest. Redis provides optional high-performance caching with SQL Server dual-write.

#### What UC18 adds

- **`JwtTokenService`** (`IJwtTokenService`) — generates and validates JWT tokens using HS256. Configured via `appsettings.json` `Jwt` section (`SecretKey`, `Issuer`, `Audience`, `ExpiryHours`)
- **`AesEncryptionService`** (`IAesEncryptionService`) — AES-256 encryption/decryption for password storage at rest. Key configured in `appsettings.json` `Encryption` section
- **`UserService`** (`IUserService`) — `RegisterAsync()` validates email uniqueness, encrypts password, persists `User`, returns JWT. `LoginAsync()` verifies encrypted password, returns JWT
- **`User` entity** (EF Core) — `Id`, `Username`, `Email`, `PasswordHash` (AES-encrypted), `CreatedAt`, with unique email constraint
- **`RegisterDto` / `LoginDto` / `AuthResponseDto`** — clean request/response contracts for auth endpoints
- **`RedisRepository`** — Redis PRIMARY + SQL Server dual-write. Reads from Redis (fast), writes to both Redis and SQL Server (durable). Falls back gracefully when Redis is offline
- **`RedisCacheService`** — wraps `IConnectionMultiplexer`. Stores serialized entities as JSON strings
- **`MemoryCacheService`** — in-process `IMemoryCache` alternative for Cache mode without Redis
- **`DisableJwtAuth` flag** — `appsettings.json` flag allows running without JWT for integration tests and development

#### Authentication flow

```
1. POST /api/v1/users/signup
   Body: { "fullName": "...", "email": "...", "password": "..." }
   → UserService.RegisterAsync()
   → AesEncryptionService.Encrypt(password) → passwordHash
   → UserRepository.Save(new User { Email, PasswordHash })
   → JwtTokenService.GenerateToken(user)
   ← 201 Created: { "token": "eyJ...", "expiresAt": "...", "userId": 1, "email": "..." }

2. POST /api/v1/users/login
   Body: { "email": "...", "password": "..." }
   → UserService.LoginAsync()
   → UserRepository.FindByEmail(email)
   → AesEncryptionService.Decrypt(passwordHash) == password
   → JwtTokenService.GenerateToken(user)
   ← 200 OK: { "token": "eyJ...", "expiresAt": "..." }

3. POST /api/v1/quantities/*
   Header: Authorization: Bearer eyJ...
   → JwtBearerMiddleware validates token
   → [Authorize] attribute on controller allows/rejects
   → 401 if missing/invalid, 200 if valid
```

#### JWT configuration — `appsettings.json`

```json
"Jwt": {
  "SecretKey":   "QuantityMeasurementApp_SuperSecretKey_AtLeast32Chars!",
  "Issuer":      "QuantityMeasurementApi",
  "Audience":    "QuantityMeasurementApiUsers",
  "ExpiryHours": "24"
},
"Encryption": {
  "Key": "QuantityMeasurementApp_AES_EncryptionKey_AtLeast32Chars!"
}
```

#### Redis dual-write mode

```json
"RepositoryType": "Redis",
"Redis": {
  "Enabled": true,
  "ConnectionString": "localhost:6379"
},
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=QuantityMeasurementDB;..."
}
```

#### Repository mode comparison

| Mode | RepositoryType | Read source | Write target | Use case |
|---|---|---|---|---|
| Cache (default) | `Cache` | In-Memory | In-Memory + JSON file | Dev / no DB |
| SQL Server | `Cache` | EF Core SQL Server | SQL Server | Production DB |
| Redis dual-write | `Redis` | Redis (fast) | Redis + SQL Server | High-performance prod |
| In-Memory (tests) | `Cache` | EF InMemory | EF InMemory | All test factories |

#### Security middleware pipeline — `Program.cs` order

```csharp
app.UseMiddleware<GlobalExceptionHandlingMiddleware>(); // catch all
app.UseDefaultFiles();    // serves wwwroot/index.html at /
app.UseStaticFiles();     // serves CSS / JS / JSON assets
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(AllowAll);
app.UseAuthentication();  // validates JWT Bearer token
app.UseAuthorization();   // enforces [Authorize] on controllers
app.MapControllers();
app.MapHealthChecks("/actuator/health");
```

#### Auth error responses

| Scenario | Status | Response |
|---|---|---|
| Missing `Authorization` header | 401 | `{ "status": 401, "error": "Unauthorized", "message": "JWT token is missing or invalid." }` |
| Invalid or expired token | 401 | `{ "status": 401, "error": "Unauthorized", "message": "JWT token is missing or invalid." }` |
| Email already registered | 409 | `{ "message": "Email already exists." }` |
| Wrong password | 401 | `{ "message": "Invalid credentials." }` |

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

## REST API Reference

### Signup

```http
POST /api/v1/users/signup
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "Pass@1234"
}
```

### Login

```http
POST /api/v1/users/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Pass@1234"
}
```

### Convert (example)

```http
POST /api/v1/quantities/convert
Authorization: Bearer <token>
Content-Type: application/json

{
  "thisQuantityDTO": { "value": 1, "unit": "FEET",   "measurementType": "LENGTH" },
  "thatQuantityDTO": { "value": 0, "unit": "INCHES", "measurementType": "LENGTH" }
}
```

Response:

```json
{
  "operation":    "CONVERT",
  "resultString": "12 INCHES",
  "measurementType": "LENGTH",
  "isError": false
}
```

### Valid measurement types and units

| MeasurementType | Valid units |
|---|---|
| `LENGTH` | `FEET`, `INCHES`, `YARDS`, `CENTIMETERS`, `METERS` |
| `WEIGHT` | `KILOGRAM`, `GRAM`, `POUND` |
| `VOLUME` | `LITRE`, `MILLILITRE`, `GALLON` |
| `TEMPERATURE` | `CELSIUS`, `FAHRENHEIT`, `KELVIN` |

> **Note:** `TEMPERATURE` does not support `add`, `subtract`, or `divide` — these return `400 Bad Request`.

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
| `DatabaseRepositoryTests.cs` | UC16 | 30 |
| `QuantityMeasurementControllerTest.cs` | UC17 | 14 |
| `QuantityMeasurementApplicationTests.cs` | UC17–UC18 | 40+ |
| **Total** | | **473+** |

All tests run in parallel via `MSTestSettings.cs` (`ExecutionScope.MethodLevel`).

---

## Build and Run

**Prerequisites:** .NET 8 SDK · SQL Server / SQL Server Express (optional) · Redis (optional)

```bash
# Clone
git clone <repository-url>
cd QuantityMeasurementApp

# Build all projects
dotnet build

# Run the API
dotnet run --project QuantityMeasurementApp/QuantityMeasurementApi

# Run all tests
dotnet test

# Run specific test project
dotnet test QuantityMeasurementApp/QuantityMeasurementApp.Tests
dotnet test QuantityMeasurementApp/QuantityMeasurementApi.Tests
```

Once running:

| URL | Description |
|---|---|
| `http://localhost:5000` | Frontend UI (served from `wwwroot/index.html`) |
| `http://localhost:5000/swagger` | Swagger UI |
| `http://localhost:5000/swagger/v1/swagger.json` | OpenAPI JSON |
| `http://localhost:5000/actuator/health` | Health check |

### Quick-start with curl

```bash
# 1. Register
curl -X POST http://localhost:5000/api/v1/users/signup \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test User","email":"test@example.com","password":"Pass@1234"}'

# 2. Copy the token from the response, then use it:
curl -X POST http://localhost:5000/api/v1/quantities/convert \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "thisQuantityDTO": {"value":1,"unit":"FEET","measurementType":"LENGTH"},
    "thatQuantityDTO": {"value":0,"unit":"INCHES","measurementType":"LENGTH"}
  }'
# → { "resultString": "12 INCHES", "operation": "CONVERT" }
```

---

## Design Principles

| Principle | How it is applied |
|---|---|
| Single Responsibility | Each layer has one job: API handles HTTP, BLL handles logic, Repository handles storage, Model holds data |
| Open / Closed | New measurement categories plug into `Quantity<TUnit>` and `IQuantityMeasurementService` without modifying existing code |
| Dependency Inversion | Controller depends on `IQuantityMeasurementService`; service depends on `IQuantityMeasurementRepository`; neither on concrete implementations |
| Interface Segregation | `ITemperatureService` exposes only equality and conversion — no arithmetic leaks through; `IQuantityMeasurementApp` exposes only `Run()` |
| DRY | UC13 centralized all arithmetic into `PerformBaseArithmetic`; UC15 centralized all operations into one service and one controller |
| Immutability | Every operation (`Add`, `Subtract`, `ConvertTo`) returns a new object; `QuantityMeasurementEntity` has no setters |
| Layer Separation | Repository stays thin — records entities without domain logic; service delegates storage after every operation |
| Default Interface Methods | UC14 uses C# default interface methods on `IMeasurable` to make arithmetic opt-out for temperature without breaking existing units |
| Functional Interfaces | Lambda `Func<double, double>` expressions implement temperature conversion formulas |
| Facade Pattern | Controllers hide service and repository complexity behind simple `PerformXxx` methods, ready to map to REST endpoints |
| Factory / Strategy | `ResolveUnit` in the service maps `QuantityOperandDto` string fields to the correct `IMeasurable` struct at runtime |
| Security | JWT Bearer validates every request to `/quantities/*`; AES-256 encrypts passwords at rest |
| Resilience | Redis dual-write with SQL Server fallback; InMemory fallback for tests; `DisableJwtAuth` flag for dev/test environments |
