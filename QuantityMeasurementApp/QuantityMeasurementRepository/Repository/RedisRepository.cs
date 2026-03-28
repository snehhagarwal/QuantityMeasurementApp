using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Context;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Interface;
using StackExchange.Redis;

namespace QuantityMeasurementRepository.Repository
{
    /// <summary>
    /// UC17: Redis Repository — Redis is the PRIMARY data store for fast reads.
    ///       SQL Server (via EF Core) is the SECONDARY durable store.
    ///
    /// WRITE path:
    ///   1. Save to SQL Server first (source of truth — row gets auto-generated Id).
    ///   2. Store JSON in Redis (fast reads, 30-minute TTL).
    ///   3. Append to Redis list keys so all filter queries stay warm.
    ///
    /// READ path:
    ///   1. Check Redis first (LRANGE on the relevant list key).
    ///   2. On cache miss → query SQL Server → populate Redis → return.
    ///
    /// Redis key layout:
    ///   qm:redis:all                     → JSON list of ALL records (newest first)
    ///   qm:redis:op:{OPERATION_TYPE}     → JSON list filtered by operation type
    ///   qm:redis:type:{CATEGORY}         → JSON list filtered by measurement category
    ///   qm:redis:errors                  → JSON list of error-only records
    ///   qm:redis:count:all               → total count as string
    ///   qm:redis:count:op:{OP}           → successful count per operation type
    ///
    /// If Redis is unreachable any method falls through silently to SQL Server.
    /// This means the app never goes down because of Redis.
    /// </summary>
    public class RedisRepository : IQuantityMeasurementEntityRepository
    {
        private readonly QuantityMeasurementDbContext   _db;
        private readonly IConnectionMultiplexer         _redis;
        private readonly ILogger<RedisRepository>       _logger;

        // ── Redis key constants ────────────────────────────────────────────────
        private const string Prefix    = "qm:redis:";
        private const string KeyAll    = Prefix + "all";
        private const string KeyErrors = Prefix + "errors";
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented               = false
        };

        public RedisRepository(
            QuantityMeasurementDbContext db,
            IConnectionMultiplexer       redis,
            ILogger<RedisRepository>     logger)
        {
            _db     = db;
            _redis  = redis;
            _logger = logger;
        }

        // ══════════════════════════════════════════════════════════════════════
        // SAVE — Write to SQL Server first, then store in Redis
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Persists a QuantityMeasurementEntity.
        /// Step 1 — SQL Server INSERT via EF Core (durable, auto-assigns Id).
        /// Step 2 — Store the serialized record in every relevant Redis list key.
        /// Step 3 — Invalidate count keys so they get recalculated on next read.
        /// If Redis fails, the record is already safe in SQL Server.
        /// </summary>
        public void Save(QuantityMeasurementEntity entity)
        {
            // ── 1. Persist to SQL Server ───────────────────────────────────
            var efRecord = MapToEfRecord(entity);
            _db.Measurements.Add(efRecord);
            _db.SaveChanges();                       // synchronous — console app context

            _logger.LogInformation(
                "RedisRepository.Save: id={Id} op={Op} → SQL Server OK.",
                efRecord.Id, efRecord.OperationType);

            // ── 2. Persist to Redis ────────────────────────────────────────
            TrySaveToRedis(entity, efRecord.OperationType);
        }

        private void TrySaveToRedis(QuantityMeasurementEntity entity, string operation)
        {
            try
            {
                var db       = _redis.GetDatabase();
                var json     = JsonSerializer.Serialize(BuildRedisRecord(entity), JsonOpts);
                var upper    = operation.ToUpperInvariant();
                var category = ExtractCategory(entity);

                // Prepend to the "all" list (newest first)
                db.ListLeftPush(KeyAll, json);
                db.KeyExpire(KeyAll, Ttl);

                // Append to the operation-type list
                var opKey = Prefix + "op:" + upper;
                db.ListLeftPush(opKey, json);
                db.KeyExpire(opKey, Ttl);

                // Append to the measurement-type list (if extractable)
                if (!string.IsNullOrEmpty(category))
                {
                    var typeKey = Prefix + "type:" + category;
                    db.ListLeftPush(typeKey, json);
                    db.KeyExpire(typeKey, Ttl);
                }

                // Append to errors list if needed
                if (entity.IsError)
                {
                    db.ListLeftPush(KeyErrors, json);
                    db.KeyExpire(KeyErrors, Ttl);
                }

                // Invalidate count keys — they will be recalculated on next read
                db.KeyDelete(Prefix + "count:all");
                db.KeyDelete(Prefix + "count:op:" + upper);

                _logger.LogDebug(
                    "RedisRepository.Save: id written to Redis lists (op={Op}, type={Type}).",
                    upper, category);
            }
            catch (System.Exception ex)
            {
                // Redis failure never shadows a successful SQL Server save
                _logger.LogWarning(ex,
                    "RedisRepository.Save: Redis write failed — data is safe in SQL Server.");
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // READ — Redis first, SQL Server fallback
        // ══════════════════════════════════════════════════════════════════════

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            var cached = TryGetListFromRedis(KeyAll);
            if (cached is not null)
            {
                _logger.LogDebug("GetAllMeasurements → Redis HIT ({N}).", cached.Count);
                return cached;
            }

            _logger.LogDebug("GetAllMeasurements → Redis MISS, querying SQL Server.");
            var records = _db.Measurements
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            var entities = records.Select(MapFromEfRecord).ToList();
            TrySetListInRedis(KeyAll, entities);
            return entities.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType)
        {
            var upper    = operationType.ToUpperInvariant();
            var redisKey = Prefix + "op:" + upper;

            var cached = TryGetListFromRedis(redisKey);
            if (cached is not null)
            {
                _logger.LogDebug("GetMeasurementsByOperationType({Op}) → Redis HIT.", upper);
                return cached;
            }

            _logger.LogDebug("GetMeasurementsByOperationType({Op}) → Redis MISS.", upper);
            var records  = _db.Measurements
                .AsNoTracking()
                .Where(e => e.OperationType == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            var entities = records.Select(MapFromEfRecord).ToList();
            TrySetListInRedis(redisKey, entities);
            return entities.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType)
        {
            var upper    = measurementType.ToUpperInvariant();
            var redisKey = Prefix + "type:" + upper;

            var cached = TryGetListFromRedis(redisKey);
            if (cached is not null)
            {
                _logger.LogDebug("GetMeasurementsByMeasurementType({Type}) → Redis HIT.", upper);
                return cached;
            }

            _logger.LogDebug("GetMeasurementsByMeasurementType({Type}) → Redis MISS.", upper);
            var records  = _db.Measurements
                .AsNoTracking()
                .Where(e => e.FirstOperandUnit != null &&
                            e.FirstOperandUnit.ToUpper() == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            var entities = records.Select(MapFromEfRecord).ToList();
            TrySetListInRedis(redisKey, entities);
            return entities.AsReadOnly();
        }

        public int GetTotalCount()
        {
            const string redisKey = Prefix + "count:all";
            try
            {
                var db  = _redis.GetDatabase();
                var val = db.StringGet(redisKey);
                if (val.HasValue && int.TryParse(val.ToString(), out var n))
                {
                    _logger.LogDebug("GetTotalCount → Redis HIT ({N}).", n);
                    return n;
                }
            }
            catch (System.Exception ex) { _logger.LogWarning(ex, "Redis GET failed for {Key}.", redisKey); }

            var count = (int)_db.Measurements.LongCount();

            try
            {
                var db = _redis.GetDatabase();
                db.StringSet(redisKey, count.ToString(), Ttl);
            }
            catch (System.Exception ex) { _logger.LogWarning(ex, "Redis SET failed for {Key}.", redisKey); }

            return count;
        }

        public void Clear()
        {
            // Clear SQL Server
            _db.Measurements.RemoveRange(_db.Measurements);
            _db.SaveChanges();

            // Flush all Redis keys for this repository
            try
            {
                var db   = _redis.GetDatabase();
                var keys = new RedisKey[]
                {
                    KeyAll, KeyErrors,
                    Prefix + "count:all",
                    Prefix + "op:COMPARE",  Prefix + "op:CONVERT",
                    Prefix + "op:ADD",      Prefix + "op:SUBTRACT",  Prefix + "op:DIVIDE",
                    Prefix + "count:op:COMPARE", Prefix + "count:op:CONVERT",
                    Prefix + "count:op:ADD",     Prefix + "count:op:SUBTRACT",
                    Prefix + "count:op:DIVIDE",
                    Prefix + "type:FEET",   Prefix + "type:INCHES",
                    Prefix + "type:KILOGRAM", Prefix + "type:GRAM",
                    Prefix + "type:LITRE",  Prefix + "type:MILLILITRE",
                    Prefix + "type:CELSIUS",Prefix + "type:FAHRENHEIT"
                };
                db.KeyDelete(keys);
                _logger.LogInformation("RedisRepository.Clear: all Redis keys deleted.");
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "Redis key delete failed during Clear().");
            }
        }

        public string GetPoolStatistics()
        {
            var total = GetTotalCount();
            string redisStatus;
            try
            {
                var db  = _redis.GetDatabase();
                db.Ping();
                redisStatus = "Connected";
            }
            catch
            {
                redisStatus = "Unavailable";
            }
            return $"RedisRepository | Redis={redisStatus} | SQL Server total records: {total}";
        }

        // ══════════════════════════════════════════════════════════════════════
        // PRIVATE REDIS HELPERS
        // ══════════════════════════════════════════════════════════════════════

        private List<QuantityMeasurementEntity>? TryGetListFromRedis(string key)
        {
            try
            {
                var db   = _redis.GetDatabase();
                var vals = db.ListRange(key);       // LRANGE key 0 -1
                if (vals.Length == 0) return null;

                var result = new List<QuantityMeasurementEntity>();
                foreach (var v in vals)
                {
                    if (!v.HasValue) continue;
                    var rec = JsonSerializer.Deserialize<RedisRecord>(v.ToString()!, JsonOpts);
                    if (rec is not null)
                        result.Add(MapFromRedisRecord(rec));
                }
                return result;
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "Redis LRANGE failed for key {Key}.", key);
                return null;
            }
        }

        private void TrySetListInRedis(string key, List<QuantityMeasurementEntity> list)
        {
            try
            {
                var db = _redis.GetDatabase();
                db.KeyDelete(key);   // clear old entries first

                foreach (var entity in list)
                {
                    var json = JsonSerializer.Serialize(BuildRedisRecord(entity), JsonOpts);
                    db.ListRightPush(key, json);     // RPUSH preserves order
                }

                if (list.Count > 0)
                    db.KeyExpire(key, Ttl);

                _logger.LogDebug("TrySetListInRedis: wrote {N} records to {Key}.", list.Count, key);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "Redis RPUSH failed for key {Key}.", key);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // MAPPING HELPERS
        // ══════════════════════════════════════════════════════════════════════

        private static QuantityMeasurementModel.Entities.QuantityMeasurement MapToEfRecord(
            QuantityMeasurementEntity entity)
        {
            return new QuantityMeasurementModel.Entities.QuantityMeasurement
            {
                OperationType       = entity.OperationType,
                FirstOperandValue   = TryParseDouble(entity.FirstOperand),
                FirstOperandUnit    = ExtractUnit(entity.FirstOperand),
                FirstOperandCategory = ExtractCategoryFromDisplay(entity.FirstOperand),
                FirstOperandDisplay = entity.FirstOperand,
                SecondOperandValue  = TryParseDouble(entity.SecondOperand),
                SecondOperandUnit   = ExtractUnit(entity.SecondOperand),
                SecondOperandCategory = ExtractCategoryFromDisplay(entity.SecondOperand),
                SecondOperandDisplay = entity.SecondOperand,
                FormattedResult     = entity.Result,
                ResultValue         = TryParseDouble(entity.Result),
                IsSuccessful        = !entity.IsError,
                ErrorDetails        = entity.ErrorMessage,
                CreatedAt           = entity.Timestamp,
                UpdatedAt           = entity.Timestamp
            };
        }

        private static QuantityMeasurementEntity MapFromEfRecord(
            QuantityMeasurementModel.Entities.QuantityMeasurement r)
        {
            if (!r.IsSuccessful)
                return new QuantityMeasurementEntity(r.OperationType,
                    (QuantityDTO?)null, (QuantityDTO?)null, r.ErrorDetails ?? "", true);

            var firstDto  = BuildDto(r.FirstOperandDisplay);
            var secondDto = BuildDto(r.SecondOperandDisplay);

            if (firstDto is not null && secondDto is not null)
                return new QuantityMeasurementEntity(r.OperationType, firstDto, secondDto,
                    r.FormattedResult ?? "");

            if (firstDto is not null)
            {
                var resultDto = BuildDto(r.FormattedResult) ?? new QuantityDTO(0, "", "");
                return new QuantityMeasurementEntity(r.OperationType, firstDto, resultDto);
            }

            return new QuantityMeasurementEntity(r.OperationType,
                (QuantityDTO?)null, (QuantityDTO?)null, r.FormattedResult ?? "", false);
        }

        private static RedisRecord BuildRedisRecord(QuantityMeasurementEntity e) => new()
        {
            OperationType  = e.OperationType,
            FirstOperand   = e.FirstOperand,
            SecondOperand  = e.SecondOperand,
            Result         = e.Result,
            IsError        = e.IsError,
            ErrorMessage   = e.ErrorMessage,
            Timestamp      = e.Timestamp.ToString("o")
        };

        private static QuantityMeasurementEntity MapFromRedisRecord(RedisRecord r)
        {
            if (r.IsError)
                return new QuantityMeasurementEntity(r.OperationType ?? "UNKNOWN",
                    (QuantityDTO?)null, (QuantityDTO?)null, r.ErrorMessage ?? "", true);

            var first  = BuildDto(r.FirstOperand);
            var second = BuildDto(r.SecondOperand);

            if (first is not null && second is not null)
                return new QuantityMeasurementEntity(r.OperationType ?? "UNKNOWN",
                    first, second, r.Result ?? "");

            if (first is not null)
            {
                var result = BuildDto(r.Result) ?? new QuantityDTO(0, "", "");
                return new QuantityMeasurementEntity(r.OperationType ?? "UNKNOWN", first, result);
            }

            return new QuantityMeasurementEntity(r.OperationType ?? "UNKNOWN",
                (QuantityDTO?)null, (QuantityDTO?)null, r.Result ?? "", false);
        }

        private static QuantityDTO? BuildDto(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            var parts = display.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            double val = parts.Length > 0 && double.TryParse(parts[0], out var v) ? v : 0;
            string unit = parts.Length > 1 ? parts[1] : "";
            return new QuantityDTO(val, unit, "UNKNOWN");
        }

        private static double? TryParseDouble(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            var first = display.Trim().Split(' ')[0];
            return double.TryParse(first, out var v) ? v : null;
        }

        private static string? ExtractUnit(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            var parts = display.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[1] : null;
        }

        private static string? ExtractCategoryFromDisplay(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            var parts = display.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 2 ? parts[2] : null;
        }

        private static string ExtractCategory(QuantityMeasurementEntity entity)
        {
            var unit = ExtractUnit(entity.FirstOperand)?.ToUpperInvariant() ?? "";
            return unit switch
            {
                "FEET" or "INCHES" or "YARDS" or "CENTIMETERS"     => "LENGTH",
                "KILOGRAM" or "GRAM" or "POUND"                     => "WEIGHT",
                "LITRE" or "MILLILITRE" or "GALLON"                 => "VOLUME",
                "CELSIUS" or "FAHRENHEIT" or "KELVIN"               => "TEMPERATURE",
                _ => unit
            };
        }

        // ── Serialization record for Redis JSON ────────────────────────────────
        private class RedisRecord
        {
            public string?  OperationType  { get; set; }
            public string?  FirstOperand   { get; set; }
            public string?  SecondOperand  { get; set; }
            public string?  Result         { get; set; }
            public bool     IsError        { get; set; }
            public string?  ErrorMessage   { get; set; }
            public string?  Timestamp      { get; set; }
        }
    }
}
