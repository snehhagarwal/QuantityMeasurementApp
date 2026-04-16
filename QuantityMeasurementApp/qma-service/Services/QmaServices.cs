using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using QmaService.Data;
using QmaService.Entities;
using QmaService.Interfaces;
using QmaService.Models;

namespace QmaService.Services;

// ── Redis Cache Service ───────────────────────────────────────────────────────

public class RedisCacheService(IConnectionMultiplexer mux, ILogger<RedisCacheService> logger) : ICacheService
{
    private const string Prefix = "qma:";
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        try
        {
            var val = await mux.GetDatabase().StringGetAsync(Prefix + key);
            return val.HasValue ? JsonSerializer.Deserialize<T>(val.ToString()!, JsonOpts) : default;
        }
        catch (Exception ex) { logger.LogWarning(ex, "Redis GET failed: {Key}", key); return default; }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOpts);
            await mux.GetDatabase().StringSetAsync(Prefix + key, json, ttl ?? TimeSpan.FromMinutes(10));
        }
        catch (Exception ex) { logger.LogWarning(ex, "Redis SET failed: {Key}", key); }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try { await mux.GetDatabase().KeyDeleteAsync(Prefix + key); }
        catch (Exception ex) { logger.LogWarning(ex, "Redis REMOVE failed: {Key}", key); }
    }
}

// ── QMA Service (calculation + history) ──────────────────────────────────────

public class QmaMeasurementService(QmaDbContext db, ICacheService cache, ILogger<QmaMeasurementService> logger) : IQmaService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    // ── Unit conversion: everything to a SI base unit ─────────────────────

    private static double ToBase(QuantityOperandDto q) => q.MeasurementType.ToUpper() switch
    {
        "LENGTH"      => q.Unit.ToUpper() switch { "FEET" => q.Value * 30.48, "INCHES" => q.Value * 2.54, "YARDS" => q.Value * 91.44, _ => q.Value },
        "WEIGHT"      => q.Unit.ToUpper() switch { "GRAM" => q.Value / 1000, "POUND" => q.Value * 0.453592, _ => q.Value },
        "VOLUME"      => q.Unit.ToUpper() switch { "MILLILITRE" => q.Value / 1000, "GALLON" => q.Value * 3.78541, _ => q.Value },
        "TEMPERATURE" => q.Unit.ToUpper() switch { "FAHRENHEIT" => (q.Value - 32) * 5 / 9, "KELVIN" => q.Value - 273.15, _ => q.Value },
        _ => q.Value
    };

    private static double FromBase(double baseVal, QuantityOperandDto target) => target.MeasurementType.ToUpper() switch
    {
        "LENGTH"      => target.Unit.ToUpper() switch { "FEET" => baseVal / 30.48, "INCHES" => baseVal / 2.54, "YARDS" => baseVal / 91.44, _ => baseVal },
        "WEIGHT"      => target.Unit.ToUpper() switch { "GRAM" => baseVal * 1000, "POUND" => baseVal / 0.453592, _ => baseVal },
        "VOLUME"      => target.Unit.ToUpper() switch { "MILLILITRE" => baseVal * 1000, "GALLON" => baseVal / 3.78541, _ => baseVal },
        "TEMPERATURE" => target.Unit.ToUpper() switch { "FAHRENHEIT" => baseVal * 9 / 5 + 32, "KELVIN" => baseVal + 273.15, _ => baseVal },
        _ => baseVal
    };

    private static void EnsureSameCategory(QuantityOperandDto a, QuantityOperandDto b, string op)
    {
        if (!string.Equals(a.MeasurementType, b.MeasurementType, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"{op}: measurement types must match ({a.MeasurementType} vs {b.MeasurementType}).");
    }

    // ── Calculation operations ────────────────────────────────────────────

    public async Task<QuantityMeasurementDto> CompareAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default)
    {
        EnsureSameCategory(first, second, "COMPARE");
        bool equal = Math.Abs(ToBase(first) - ToBase(second)) < 1e-4;
        var result = equal.ToString().ToLowerInvariant();
        return await PersistAsync("COMPARE", first, second, result, equal ? 1d : 0d, "RESULT", null, true, null, userId, ct);
    }

    public async Task<QuantityMeasurementDto> ConvertAsync(QuantityOperandDto quantity, QuantityOperandDto target, long? userId, CancellationToken ct = default)
    {
        EnsureSameCategory(quantity, target, "CONVERT");
        double converted = Math.Round(FromBase(ToBase(quantity), target), 4);
        return await PersistAsync("CONVERT", quantity, target, $"{converted} {target.Unit.ToUpper()}", converted, target.Unit.ToUpper(), target.MeasurementType.ToUpper(), true, null, userId, ct);
    }

    public async Task<QuantityMeasurementDto> AddAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default)
    {
        EnsureSameCategory(first, second, "ADD");
        double result = Math.Round(FromBase(ToBase(first) + ToBase(second), first), 4);
        return await PersistAsync("ADD", first, second, $"{result} {first.Unit.ToUpper()}", result, first.Unit.ToUpper(), first.MeasurementType.ToUpper(), true, null, userId, ct);
    }

    public async Task<QuantityMeasurementDto> SubtractAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default)
    {
        EnsureSameCategory(first, second, "SUBTRACT");
        double result = Math.Round(FromBase(ToBase(first) - ToBase(second), first), 4);
        return await PersistAsync("SUBTRACT", first, second, $"{result} {first.Unit.ToUpper()}", result, first.Unit.ToUpper(), first.MeasurementType.ToUpper(), true, null, userId, ct);
    }

    public async Task<QuantityMeasurementDto> DivideAsync(QuantityOperandDto first, QuantityOperandDto second, long? userId, CancellationToken ct = default)
    {
        EnsureSameCategory(first, second, "DIVIDE");
        double baseB = ToBase(second);
        if (Math.Abs(baseB) < 1e-10) throw new DivideByZeroException("Cannot divide by zero.");
        double result = Math.Round(ToBase(first) / baseB, 4);
        return await PersistAsync("DIVIDE", first, second, result.ToString("G"), result, "RATIO", first.MeasurementType.ToUpper(), true, null, userId, ct);
    }

    // ── History queries ───────────────────────────────────────────────────

    public async Task<IEnumerable<QuantityMeasurementDto>> GetHistoryByUserAsync(long userId, CancellationToken ct = default)
    {
        var cacheKey = $"history:user:{userId}";
        var cached = await cache.GetAsync<List<QuantityMeasurementDto>>(cacheKey, ct);
        if (cached != null) return cached;

        var records = await db.Measurements
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => ToDto(m))
            .ToListAsync(ct);

        await cache.SetAsync(cacheKey, records, CacheTtl, ct);
        return records;
    }

    public async Task<IEnumerable<QuantityMeasurementDto>> GetHistoryByOperationTypeAndUserAsync(string operationType, long userId, CancellationToken ct = default)
    {
        var op = operationType.ToUpper();
        return await db.Measurements
            .Where(m => m.UserId == userId && m.OperationType == op)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => ToDto(m))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<QuantityMeasurementDto>> GetHistoryByMeasurementTypeAsync(string measurementType, CancellationToken ct = default)
    {
        var mt = measurementType.ToUpper();
        return await db.Measurements
            .Where(m => m.FirstOperandCategory == mt)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => ToDto(m))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<QuantityMeasurementDto>> GetErrorHistoryByUserAsync(long userId, CancellationToken ct = default)
        => await db.Measurements
            .Where(m => m.UserId == userId && !m.IsSuccessful)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => ToDto(m))
            .ToListAsync(ct);

    public async Task<int> ClearHistoryByUserAsync(long userId, CancellationToken ct = default)
    {
        var records = await db.Measurements.Where(m => m.UserId == userId).ToListAsync(ct);
        db.Measurements.RemoveRange(records);
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync($"history:user:{userId}", ct);
        return records.Count;
    }

    public async Task<int> CountByOperationTypeAsync(string operationType, CancellationToken ct = default)
        => await db.Measurements.CountAsync(m => m.OperationType == operationType.ToUpper() && m.IsSuccessful, ct);

    // ── Helpers ───────────────────────────────────────────────────────────

    private async Task<QuantityMeasurementDto> PersistAsync(
        string op, QuantityOperandDto first, QuantityOperandDto second,
        string formatted, double? resultValue, string? resultUnit, string? resultType,
        bool success, string? error, long? userId, CancellationToken ct)
    {
        var entity = new QuantityMeasurement
        {
            OperationType        = op,
            FirstOperandValue    = first.Value,
            FirstOperandUnit     = first.Unit.ToUpper(),
            FirstOperandCategory = first.MeasurementType.ToUpper(),
            FirstOperandDisplay  = $"{first.Value} {first.Unit.ToUpper()}",
            SecondOperandValue   = second.Value,
            SecondOperandUnit    = second.Unit.ToUpper(),
            SecondOperandCategory = second.MeasurementType.ToUpper(),
            SecondOperandDisplay = $"{second.Value} {second.Unit.ToUpper()}",
            TargetUnit           = op == "CONVERT" ? second.Unit.ToUpper() : null,
            FormattedResult      = formatted,
            ResultValue          = resultValue,
            ResultUnit           = resultUnit,
            ResultMeasurementType = resultType,
            IsSuccessful         = success,
            ErrorDetails         = error,
            UserId               = userId,
            CreatedAt            = DateTime.UtcNow,
            UpdatedAt            = DateTime.UtcNow
        };

        db.Measurements.Add(entity);
        await db.SaveChangesAsync(ct);

        if (userId.HasValue)
            await cache.RemoveAsync($"history:user:{userId}", ct);

        logger.LogInformation("[QMA] {Op} saved id={Id}", op, entity.Id);
        return ToDto(entity);
    }

    private static QuantityMeasurementDto ToDto(QuantityMeasurement m) => new()
    {
        Id                   = m.Id,
        OperationType        = m.OperationType,
        FormattedResult      = m.FormattedResult ?? string.Empty,
        ResultValue          = m.ResultValue,
        ResultUnit           = m.ResultUnit,
        ResultMeasurementType = m.ResultMeasurementType,
        IsSuccessful         = m.IsSuccessful,
        ErrorDetails         = m.ErrorDetails,
        UserId               = m.UserId,
        CreatedAt            = m.CreatedAt,
        FirstOperandValue    = m.FirstOperandValue,
        FirstOperandUnit     = m.FirstOperandUnit,
        FirstOperandCategory = m.FirstOperandCategory,
        SecondOperandValue   = m.SecondOperandValue,
        SecondOperandUnit    = m.SecondOperandUnit,
        SecondOperandCategory = m.SecondOperandCategory,
        TargetUnit           = m.TargetUnit
    };
}
