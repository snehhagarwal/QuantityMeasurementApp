using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Context;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;
using StackExchange.Redis;

namespace QuantityMeasurementRepository.Service
{
    /// <summary>
    /// UC17: Redis-first repository for QuantityMeasurement records.
    ///
    /// WRITE path → SQL Server first (source of truth via EF Core), then invalidate Redis.
    /// READ  path → Redis first; on cache miss, query SQL Server and populate Redis.
    ///
    /// Replaces the old ADO.NET QuantityMeasurementDatabaseRepository (stored procedures,
    /// manual SqlConnection) and the old QuantityMeasurementCacheRepository (in-memory list
    /// + JSON file). No stored procedures. No ADO.NET. No SqlConnection.
    ///
    /// Redis key layout (all prefixed "qm:repo:"):
    ///   qm:repo:all               → full measurement list (JSON array)
    ///   qm:repo:errors            → error-only list
    ///   qm:repo:op:{OP}           → list filtered by operation type
    ///   qm:repo:type:{TYPE}       → list filtered by measurement type
    ///   qm:repo:count:op:{OP}     → successful count for an operation type
    ///   qm:repo:count:all         → total record count
    /// </summary>
    public class QuantityMeasurementRepository : IQuantityMeasurementRepository
    {
        private readonly QuantityMeasurementDbContext            _db;
        private readonly IConnectionMultiplexer?                 _redis;   // null when Redis disabled
        private readonly ILogger<QuantityMeasurementRepository>  _logger;

        private const string Prefix    = "qm:repo:";
        private const string KeyAll    = Prefix + "all";
        private const string KeyErrors = Prefix + "errors";
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented               = false
        };

        public QuantityMeasurementRepository(
            QuantityMeasurementDbContext             db,
            ILogger<QuantityMeasurementRepository>  logger,
            IConnectionMultiplexer?                 redis = null)
        {
            _db     = db;
            _logger = logger;
            _redis  = redis;
        }

        // ── SAVE: SQL Server first, then invalidate Redis ──────────────────────

        public async Task<QuantityMeasurement> SaveAsync(
            QuantityMeasurement entity,
            CancellationToken   cancellationToken = default)
        {
            // Step 1 — persist to SQL Server (source of truth)
            _db.Measurements.Add(entity);
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation(
                "SaveAsync: id={Id} op={Op} written to SQL Server.", entity.Id, entity.OperationType);

            // Step 2 — invalidate stale Redis keys (never throws)
            await InvalidateRedisAsync(entity).ConfigureAwait(false);
            return entity;
        }

        // ── FIND ALL ───────────────────────────────────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurement>> FindAllAsync(
            CancellationToken cancellationToken = default)
        {
            var cached = await RedisGetListAsync(KeyAll).ConfigureAwait(false);
            if (cached is not null)
            {
                _logger.LogDebug("FindAllAsync → Redis HIT ({N} records).", cached.Count);
                return cached;
            }
            _logger.LogDebug("FindAllAsync → Redis MISS, hitting SQL Server.");
            var list = await _db.Measurements
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            await RedisSetListAsync(KeyAll, list).ConfigureAwait(false);
            return list.AsReadOnly();
        }

        // ── FIND BY OPERATION TYPE ─────────────────────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByOperationTypeAsync(
            string            operationType,
            CancellationToken cancellationToken = default)
        {
            var upper    = operationType.ToUpperInvariant();
            var redisKey = Prefix + "op:" + upper;

            var cached = await RedisGetListAsync(redisKey).ConfigureAwait(false);
            if (cached is not null)
            {
                _logger.LogDebug("FindByOperationType({Op}) → Redis HIT.", upper);
                return cached;
            }
            _logger.LogDebug("FindByOperationType({Op}) → Redis MISS, hitting SQL Server.", upper);
            var list = await _db.Measurements
                .AsNoTracking()
                .Where(e => e.OperationType == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            await RedisSetListAsync(redisKey, list).ConfigureAwait(false);
            return list.AsReadOnly();
        }

        // ── FIND BY MEASUREMENT TYPE ───────────────────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByMeasurementTypeAsync(
            string            measurementType,
            CancellationToken cancellationToken = default)
        {
            var upper    = measurementType.ToUpperInvariant();
            var redisKey = Prefix + "type:" + upper;

            var cached = await RedisGetListAsync(redisKey).ConfigureAwait(false);
            if (cached is not null)
            {
                _logger.LogDebug("FindByMeasurementType({Type}) → Redis HIT.", upper);
                return cached;
            }
            _logger.LogDebug("FindByMeasurementType({Type}) → Redis MISS, hitting SQL Server.", upper);
            var list = await _db.Measurements
                .AsNoTracking()
                .Where(e => e.FirstOperandCategory != null && e.FirstOperandCategory == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            await RedisSetListAsync(redisKey, list).ConfigureAwait(false);
            return list.AsReadOnly();
        }

        // ── FIND BY DATE (not cached — too dynamic) ────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByCreatedAtAfterAsync(
            DateTime          after,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("FindByCreatedAtAfter({After}) → SQL Server (not cached).", after);
            return await _db.Measurements
                .AsNoTracking()
                .Where(e => e.CreatedAt > after)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        // ── FIND ERRORS ────────────────────────────────────────────────────────

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByIsErrorTrueAsync(
            CancellationToken cancellationToken = default)
        {
            var cached = await RedisGetListAsync(KeyErrors).ConfigureAwait(false);
            if (cached is not null)
            {
                _logger.LogDebug("FindByIsErrorTrue → Redis HIT ({N} records).", cached.Count);
                return cached;
            }
            _logger.LogDebug("FindByIsErrorTrue → Redis MISS, hitting SQL Server.");
            var list = await _db.Measurements
                .AsNoTracking()
                .Where(e => !e.IsSuccessful)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            await RedisSetListAsync(KeyErrors, list).ConfigureAwait(false);
            return list.AsReadOnly();
        }

        // ── COUNT BY OPERATION ─────────────────────────────────────────────────

        public async Task<long> CountByOperationTypeAndIsErrorFalseAsync(
            string            operationType,
            CancellationToken cancellationToken = default)
        {
            var upper    = operationType.ToUpperInvariant();
            var redisKey = Prefix + "count:op:" + upper;

            if (_redis is not null)
            {
                try
                {
                    var rdb = _redis.GetDatabase();
                    var val = await rdb.StringGetAsync(redisKey).ConfigureAwait(false);
                    if (val.HasValue && long.TryParse(val.ToString(), out var n))
                    {
                        _logger.LogDebug("CountByOperation({Op}) → Redis HIT ({N}).", upper, n);
                        return n;
                    }
                }
                catch (System.Exception ex) { _logger.LogWarning(ex, "Redis GET failed: {Key}.", redisKey); }
            }

            _logger.LogDebug("CountByOperation({Op}) → Redis MISS, hitting SQL Server.", upper);
            var count = await _db.Measurements
                .LongCountAsync(e => e.OperationType == upper && e.IsSuccessful, cancellationToken)
                .ConfigureAwait(false);

            if (_redis is not null)
            {
                try
                {
                    var rdb = _redis.GetDatabase();
                    await rdb.StringSetAsync(redisKey, count.ToString(), Ttl).ConfigureAwait(false);
                }
                catch (System.Exception ex) { _logger.LogWarning(ex, "Redis SET failed: {Key}.", redisKey); }
            }
            return count;
        }

        // ── TOTAL COUNT ────────────────────────────────────────────────────────

        public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        {
            const string redisKey = Prefix + "count:all";

            if (_redis is not null)
            {
                try
                {
                    var rdb = _redis.GetDatabase();
                    var val = await rdb.StringGetAsync(redisKey).ConfigureAwait(false);
                    if (val.HasValue && long.TryParse(val.ToString(), out var n))
                    {
                        _logger.LogDebug("CountAsync → Redis HIT ({N}).", n);
                        return n;
                    }
                }
                catch (System.Exception ex) { _logger.LogWarning(ex, "Redis GET failed: {Key}.", redisKey); }
            }

            var count = await _db.Measurements.LongCountAsync(cancellationToken).ConfigureAwait(false);

            if (_redis is not null)
            {
                try
                {
                    var rdb = _redis.GetDatabase();
                    await rdb.StringSetAsync(redisKey, count.ToString(), Ttl).ConfigureAwait(false);
                }
                catch (System.Exception ex) { _logger.LogWarning(ex, "Redis SET failed: {Key}.", redisKey); }
            }
            return count;
        }

        // ── PRIVATE REDIS HELPERS ──────────────────────────────────────────────

        /// <summary>GET a JSON array from Redis and deserialize it. Returns null on miss or error.</summary>
        private async Task<List<QuantityMeasurement>?> RedisGetListAsync(string key)
        {
            if (_redis is null) return null;
            try
            {
                var db  = _redis.GetDatabase();
                var val = await db.StringGetAsync(key).ConfigureAwait(false);
                if (!val.HasValue) return null;
                return JsonSerializer.Deserialize<List<QuantityMeasurement>>(val.ToString()!, JsonOpts);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "Redis GET failed for key {Key}.", key);
                return null;
            }
        }

        /// <summary>Serialize a list to JSON and SET it in Redis with TTL. Never throws.</summary>
        private async Task RedisSetListAsync(string key, IEnumerable<QuantityMeasurement> list)
        {
            if (_redis is null) return;
            try
            {
                var db   = _redis.GetDatabase();
                var json = JsonSerializer.Serialize(list, JsonOpts);
                await db.StringSetAsync(key, json, Ttl).ConfigureAwait(false);
                _logger.LogDebug("Redis SET {Key} TTL={Ttl}.", key, Ttl);
            }
            catch (System.Exception ex) { _logger.LogWarning(ex, "Redis SET failed for key {Key}.", key); }
        }

        /// <summary>
        /// Delete all Redis keys that could be stale after a new record is saved.
        /// Uses a single DEL command with multiple keys — one round-trip to Redis.
        /// Never throws — a Redis failure must not shadow a successful SQL Server write.
        /// </summary>
        private async Task InvalidateRedisAsync(QuantityMeasurement entity)
        {
            if (_redis is null) return;
            try
            {
                var db    = _redis.GetDatabase();
                var upper = entity.OperationType.ToUpperInvariant();
                var type  = (entity.FirstOperandCategory ?? string.Empty).ToUpperInvariant();

                var keys = new List<RedisKey>
                {
                    KeyAll,
                    Prefix + "op:"       + upper,
                    Prefix + "count:op:" + upper,
                    Prefix + "count:all"
                };

                if (!string.IsNullOrEmpty(type))
                    keys.Add(Prefix + "type:" + type);

                if (!entity.IsSuccessful)
                    keys.Add(KeyErrors);

                await db.KeyDeleteAsync(keys.ToArray()).ConfigureAwait(false);

                _logger.LogInformation(
                    "Redis invalidated {Count} keys after save (op={Op}, type={Type}).",
                    keys.Count, upper, type);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis invalidation failed after SaveAsync — data is safe in SQL Server.");
            }
        }
    }
}