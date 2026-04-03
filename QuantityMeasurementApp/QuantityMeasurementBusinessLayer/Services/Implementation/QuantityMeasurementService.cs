using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Exceptions;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementBusinessLayer.Services.Interface;
using System.Security.Claims;

namespace QuantityMeasurementBusinessLayer.Services.Implementation
{
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;
        private readonly ILogger<QuantityMeasurementService> _logger;
        private readonly ICacheService _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string CacheKeyAllHistory = "history:all";
        private const string CacheKeyErrorHistory = "history:errored";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

        public QuantityMeasurementService(
            IQuantityMeasurementRepository repository,
            ILogger<QuantityMeasurementService> logger,
            ICacheService cache,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        // ── Get current user ID from JWT claims (null if anonymous) ──────────
        private long? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                       ?? user.FindFirst("sub")
                       ?? user.FindFirst("id")
                       ?? user.FindFirst("userId");
            if (idClaim == null) return null;
            return long.TryParse(idClaim.Value, out var id) ? id : null;
        }

        public async Task<QuantityMeasurementDto> CompareAsync(QuantityOperandDto first, QuantityOperandDto second,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("COMPARE {First} {FirstUnit} vs {Second} {SecondUnit}",
                first.Value, first.Unit, second.Value, second.Unit);
            try
            {
                ValidateSameCategory(first, second, "COMPARE");
                bool equal = Math.Abs(ToBaseUnit(first) - ToBaseUnit(second)) < 1e-4;
                var dto = await PersistSuccessAsync("COMPARE", first, second,
                    equal.ToString().ToLowerInvariant(),
                    equal ? 1 : 0,
                    "RESULT",
                    null,
                    cancellationToken).ConfigureAwait(false);
                await ClearCacheAsync(cancellationToken).ConfigureAwait(false);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "COMPARE unexpected error");
                await PersistErrorAsync("COMPARE", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException("Compare failed: " + ex.Message, ex);
            }
        }

        public async Task<QuantityMeasurementDto> ConvertAsync(QuantityOperandDto quantity, QuantityOperandDto target,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("CONVERT {Value} {Unit} to {TargetUnit}", quantity.Value, quantity.Unit, target.Unit);
            try
            {
                ValidateSameCategory(quantity, target, "CONVERT");
                double converted = Math.Round(FromBaseUnit(ToBaseUnit(quantity), target), 4);
                string resultStr = $"{converted} {target.Unit.ToUpperInvariant()}";
                var dto = await PersistSuccessAsync("CONVERT", quantity, target,
                    resultStr, converted,
                    target.Unit.ToUpperInvariant(),
                    target.MeasurementType.ToUpperInvariant(),
                    cancellationToken).ConfigureAwait(false);
                await ClearCacheAsync(cancellationToken).ConfigureAwait(false);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CONVERT unexpected error");
                await PersistErrorAsync("CONVERT", quantity, target, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException("Convert failed: " + ex.Message, ex);
            }
        }

        public async Task<QuantityMeasurementDto> AddAsync(QuantityOperandDto first, QuantityOperandDto second,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ADD {First} {FirstUnit} + {Second} {SecondUnit}",
                first.Value, first.Unit, second.Value, second.Unit);
            try
            {
                ValidateSameCategory(first, second, "ADD");
                ValidateArithmeticSupported(first, "Add");
                double result = Math.Round(FromBaseUnit(ToBaseUnit(first) + ToBaseUnit(second), first), 4);
                string resultStr = $"{result} {first.Unit.ToUpperInvariant()}";
                var dto = await PersistSuccessAsync("ADD", first, second,
                    resultStr, result,
                    first.Unit.ToUpperInvariant(),
                    first.MeasurementType.ToUpperInvariant(),
                    cancellationToken).ConfigureAwait(false);
                await ClearCacheAsync(cancellationToken).ConfigureAwait(false);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning("ADD not supported: {Message}", ex.Message);
                await PersistErrorAsync("ADD", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ADD unexpected error");
                await PersistErrorAsync("ADD", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException("Add failed: " + ex.Message, ex);
            }
        }

        public async Task<QuantityMeasurementDto> SubtractAsync(QuantityOperandDto first, QuantityOperandDto second,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("SUBTRACT {First} {FirstUnit} - {Second} {SecondUnit}",
                first.Value, first.Unit, second.Value, second.Unit);
            try
            {
                ValidateSameCategory(first, second, "SUBTRACT");
                ValidateArithmeticSupported(first, "Subtract");
                double result = Math.Round(FromBaseUnit(ToBaseUnit(first) - ToBaseUnit(second), first), 4);
                string resultStr = $"{result} {first.Unit.ToUpperInvariant()}";
                var dto = await PersistSuccessAsync("SUBTRACT", first, second,
                    resultStr, result,
                    first.Unit.ToUpperInvariant(),
                    first.MeasurementType.ToUpperInvariant(),
                    cancellationToken).ConfigureAwait(false);
                await ClearCacheAsync(cancellationToken).ConfigureAwait(false);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning("SUBTRACT not supported: {Message}", ex.Message);
                await PersistErrorAsync("SUBTRACT", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SUBTRACT unexpected error");
                await PersistErrorAsync("SUBTRACT", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException("Subtract failed: " + ex.Message, ex);
            }
        }

        public async Task<QuantityMeasurementDto> DivideAsync(QuantityOperandDto first, QuantityOperandDto second,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DIVIDE {First} {FirstUnit} / {Second} {SecondUnit}",
                first.Value, first.Unit, second.Value, second.Unit);
            try
            {
                ValidateSameCategory(first, second, "DIVIDE");
                ValidateArithmeticSupported(first, "Divide");
                double baseB = ToBaseUnit(second);
                if (Math.Abs(baseB) < 1e-12)
                {
                    _logger.LogError("DIVIDE by zero");
                    throw new ArithmeticException("Divide by zero");
                }
                double ratio = Math.Round(ToBaseUnit(first) / baseB, 4);
                string resultStr = ratio.ToString("G");
                var dto = await PersistSuccessAsync("DIVIDE", first, second,
                    resultStr, ratio, null, "DIMENSIONLESS",
                    cancellationToken).ConfigureAwait(false);
                await ClearCacheAsync(cancellationToken).ConfigureAwait(false);
                return dto;
            }
            catch (QuantityMeasurementException) { throw; }
            catch (ArithmeticException) { throw; }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning("DIVIDE not supported: {Message}", ex.Message);
                await PersistErrorAsync("DIVIDE", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DIVIDE unexpected error");
                await PersistErrorAsync("DIVIDE", first, second, ex.Message, cancellationToken).ConfigureAwait(false);
                throw new QuantityMeasurementException("Divide failed: " + ex.Message, ex);
            }
        }

        // ── History — ALL (admin use, no user filter) ─────────────────────────
        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetAllHistoryAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindAllAsync(cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        // ── History — scoped to logged-in user ────────────────────────────────
        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindAllByUserAsync(userId, cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByOperationTypeAsync(string op,
            CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindByOperationTypeAsync(op, cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByOperationTypeAndUserAsync(string op, long userId,
            CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindByOperationTypeAndUserAsync(op, userId, cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetHistoryByMeasurementTypeAsync(string type,
            CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindByMeasurementTypeAsync(type, cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetErrorHistoryAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindByIsErrorTrueAsync(cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        public async Task<IReadOnlyList<QuantityMeasurementDto>> GetErrorHistoryByUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            var list = await _repository.FindByIsErrorTrueAndUserAsync(userId, cancellationToken).ConfigureAwait(false);
            return QuantityMeasurementDto.FromEntityList(list).AsReadOnly();
        }

        public Task<long> CountByOperationTypeAsync(string op, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("CountByOperationType({Op})", op);
            return _repository.CountByOperationTypeAndIsErrorFalseAsync(op, cancellationToken);
        }

        public async Task<int> ClearHistoryByUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ClearHistoryByUser({UserId})", userId);
            var deleted = await _repository.DeleteAllByUserAsync(userId, cancellationToken).ConfigureAwait(false);
            await ClearCacheAsync(cancellationToken).ConfigureAwait(false);
            return deleted;
        }

        // ── Private helpers ───────────────────────────────────────────────────
        private async Task ClearCacheAsync(CancellationToken cancellationToken)
        {
            var keys = new[]
            {
                CacheKeyAllHistory, CacheKeyErrorHistory,
                "history:operation:COMPARE", "history:operation:CONVERT",
                "history:operation:ADD", "history:operation:SUBTRACT",
                "history:operation:DIVIDE",
                "history:type:LENGTH", "history:type:WEIGHT",
                "history:type:VOLUME", "history:type:TEMPERATURE"
            };
            foreach (var key in keys)
                await _cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
        }

        private static double ToBaseUnit(QuantityOperandDto op) =>
            op.MeasurementType.ToUpperInvariant() switch
            {
                "LENGTH"      => LengthToBase(op.Value, op.Unit.ToUpperInvariant()),
                "WEIGHT"      => WeightToBase(op.Value, op.Unit.ToUpperInvariant()),
                "VOLUME"      => VolumeToBase(op.Value, op.Unit.ToUpperInvariant()),
                "TEMPERATURE" => TempToBase(op.Value, op.Unit.ToUpperInvariant()),
                _ => throw new QuantityMeasurementException($"Unknown measurementType: {op.MeasurementType}")
            };

        private static double FromBaseUnit(double b, QuantityOperandDto target) =>
            target.MeasurementType.ToUpperInvariant() switch
            {
                "LENGTH"      => LengthFromBase(b, target.Unit.ToUpperInvariant()),
                "WEIGHT"      => WeightFromBase(b, target.Unit.ToUpperInvariant()),
                "VOLUME"      => VolumeFromBase(b, target.Unit.ToUpperInvariant()),
                "TEMPERATURE" => TempFromBase(b, target.Unit.ToUpperInvariant()),
                _ => throw new QuantityMeasurementException($"Unknown measurementType: {target.MeasurementType}")
            };

        private static double LengthToBase(double v, string u) => u switch
        {
            "FEET" => v, "INCHES" => v / 12.0, "YARDS" => v * 3.0, "CENTIMETERS" => v / 30.48,
            _ => throw new QuantityMeasurementException($"Unknown LENGTH unit: {u}")
        };
        private static double LengthFromBase(double b, string u) => u switch
        {
            "FEET" => b, "INCHES" => b * 12.0, "YARDS" => b / 3.0, "CENTIMETERS" => b * 30.48,
            _ => throw new QuantityMeasurementException($"Unknown LENGTH unit: {u}")
        };
        private static double WeightToBase(double v, string u) => u switch
        {
            "KILOGRAM" => v, "GRAM" => v / 1000.0, "POUND" => v * 0.453592,
            _ => throw new QuantityMeasurementException($"Unknown WEIGHT unit: {u}")
        };
        private static double WeightFromBase(double b, string u) => u switch
        {
            "KILOGRAM" => b, "GRAM" => b * 1000.0, "POUND" => b / 0.453592,
            _ => throw new QuantityMeasurementException($"Unknown WEIGHT unit: {u}")
        };
        private static double VolumeToBase(double v, string u) => u switch
        {
            "LITRE" => v, "MILLILITRE" => v / 1000.0, "GALLON" => v * 3.78541,
            _ => throw new QuantityMeasurementException($"Unknown VOLUME unit: {u}")
        };
        private static double VolumeFromBase(double b, string u) => u switch
        {
            "LITRE" => b, "MILLILITRE" => b * 1000.0, "GALLON" => b / 3.78541,
            _ => throw new QuantityMeasurementException($"Unknown VOLUME unit: {u}")
        };
        private static double TempToBase(double v, string u) => u switch
        {
            "CELSIUS" => v, "FAHRENHEIT" => (v - 32.0) * 5.0 / 9.0, "KELVIN" => v - 273.15,
            _ => throw new QuantityMeasurementException($"Unknown TEMPERATURE unit: {u}")
        };
        private static double TempFromBase(double b, string u) => u switch
        {
            "CELSIUS" => b, "FAHRENHEIT" => (b * 9.0 / 5.0) + 32.0, "KELVIN" => b + 273.15,
            _ => throw new QuantityMeasurementException($"Unknown TEMPERATURE unit: {u}")
        };

        private static void ValidateSameCategory(QuantityOperandDto a, QuantityOperandDto b, string op)
        {
            if (!string.Equals(a.MeasurementType, b.MeasurementType, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException(
                    $"{op} Error: Cannot mix {a.MeasurementType} and {b.MeasurementType}");
        }

        private static void ValidateArithmeticSupported(QuantityOperandDto op, string operation)
        {
            if (op.MeasurementType.ToUpperInvariant() == "TEMPERATURE")
                throw new NotSupportedException($"Temperature does not support {operation}.");
        }

        private async Task<QuantityMeasurementDto> PersistSuccessAsync(
            string operation, QuantityOperandDto first, QuantityOperandDto second,
            string resultStr, double resultValue, string? resultUnit, string? resultMeasType,
            CancellationToken cancellationToken)
        {
            var entity = new QuantityMeasurement
            {
                OperationType         = operation,
                UserId                = GetCurrentUserId(),
                FirstOperandValue     = first.Value,
                FirstOperandUnit      = first.Unit.ToUpperInvariant(),
                FirstOperandCategory  = first.MeasurementType.ToUpperInvariant(),
                FirstOperandDisplay   = $"{first.Value} {first.Unit.ToUpperInvariant()}",
                SecondOperandValue    = second.Value,
                SecondOperandUnit     = second.Unit.ToUpperInvariant(),
                SecondOperandCategory = second.MeasurementType.ToUpperInvariant(),
                SecondOperandDisplay  = $"{second.Value} {second.Unit.ToUpperInvariant()}",
                TargetUnit            = operation == "CONVERT" ? second.Unit.ToUpperInvariant() : null,
                FormattedResult       = resultStr,
                ResultValue           = resultValue,
                ResultUnit            = resultUnit,
                ResultMeasurementType = resultMeasType,
                IsSuccessful          = true,
                CreatedAt             = DateTime.UtcNow,
                UpdatedAt             = DateTime.UtcNow
            };
            var saved = await _repository.SaveAsync(entity, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Persisted {Operation} → {Result} (userId={UserId})", operation, resultStr, entity.UserId);
            return QuantityMeasurementDto.FromEntity(saved);
        }

        private async Task PersistErrorAsync(string operation, QuantityOperandDto first,
            QuantityOperandDto second, string errorMessage, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.SaveAsync(new QuantityMeasurement
                {
                    OperationType         = operation,
                    UserId                = GetCurrentUserId(),
                    FirstOperandValue     = first.Value,
                    FirstOperandUnit      = first.Unit.ToUpperInvariant(),
                    FirstOperandCategory  = first.MeasurementType.ToUpperInvariant(),
                    FirstOperandDisplay   = $"{first.Value} {first.Unit.ToUpperInvariant()}",
                    SecondOperandValue    = second.Value,
                    SecondOperandUnit     = second.Unit.ToUpperInvariant(),
                    SecondOperandCategory = second.MeasurementType.ToUpperInvariant(),
                    SecondOperandDisplay  = $"{second.Value} {second.Unit.ToUpperInvariant()}",
                    IsSuccessful          = false,
                    ErrorDetails          = errorMessage,
                    CreatedAt             = DateTime.UtcNow,
                    UpdatedAt             = DateTime.UtcNow
                }, cancellationToken).ConfigureAwait(false);
                _logger.LogWarning("Persisted failed {Operation}: {Error}", operation, errorMessage);
            }
            catch { /* never shadow the original exception */ }
        }
    }
}
