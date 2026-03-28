using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Interface;

namespace QuantityMeasurementRepository.Repository
{
    /// <summary>
    /// UC17: Cache Repository — In-Memory list + JSON file persistence.
    ///
    /// WRITE path:
    ///   1. Append record to the in-memory list.
    ///   2. Flush the list to a JSON file on disk (Data/measurements_cache.json).
    ///
    /// READ path:
    ///   1. Serve directly from the in-memory list.
    ///   2. On first start (empty list), load from JSON file if it exists.
    ///
    /// No database, no ADO.NET, no EF Core — pure in-process cache with a JSON fallback.
    /// </summary>
    public class CacheRepository : IQuantityMeasurementEntityRepository
    {
        private readonly List<QuantityMeasurementEntity>    _cache = new();
        private readonly ILogger<CacheRepository>           _logger;
        private readonly string                             _jsonPath;
        private static readonly JsonSerializerOptions       JsonOpts = new() { WriteIndented = true };
        private readonly object                             _lock = new();

        public CacheRepository(ILogger<CacheRepository> logger)
        {
            _logger   = logger;
            var baseDir= AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            _jsonPath= Path.Combine(projectDir, "quantity_measurements.json");
            LoadFromJsonFile();
        }

        // ── SAVE ──────────────────────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            lock (_lock)
            {
                _cache.Insert(0, entity);   // newest first
                _logger.LogInformation("CacheRepository.Save: {Op} stored in memory (total={N}).",
                    entity.OperationType, _cache.Count);
                FlushToJsonFile();
            }
        }

        // ── READ ──────────────────────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            lock (_lock) { return _cache.AsReadOnly(); }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType)
        {
            var upper = operationType.ToUpperInvariant();
            lock (_lock)
            {
                return _cache
                    .Where(e => e.OperationType.ToUpperInvariant() == upper)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType)
        {
            var upper = measurementType.ToUpperInvariant();
            lock (_lock)
            {
                return _cache
                    .Where(e => ExtractUnit(e.FirstOperand)?.ToUpperInvariant() == upper)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public int GetTotalCount()
        {
            lock (_lock) { return _cache.Count; }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                FlushToJsonFile();
                _logger.LogInformation("CacheRepository.Clear: in-memory cache and JSON file cleared.");
            }
        }

        public string GetPoolStatistics()
            => $"CacheRepository (In-Memory + JSON) | Total records: {GetTotalCount()} | File: {_jsonPath}";

        // ── JSON PERSISTENCE ──────────────────────────────────────────────────

        private void FlushToJsonFile()
        {
            try
            {
                var dir = Path.GetDirectoryName(_jsonPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonSerializer.Serialize(_cache, JsonOpts);
                File.WriteAllText(_jsonPath, json);
                _logger.LogDebug("CacheRepository: flushed {N} records to {Path}.", _cache.Count, _jsonPath);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "CacheRepository: JSON flush failed — data is safe in memory.");
            }
        }

        private void LoadFromJsonFile()
        {
            try
            {
                if (!File.Exists(_jsonPath)) return;
                var json     = File.ReadAllText(_jsonPath);
                var loaded   = JsonSerializer.Deserialize<List<QuantityMeasurementEntity>>(json, JsonOpts);
                if (loaded is { Count: > 0 })
                {
                    _cache.AddRange(loaded);
                    _logger.LogInformation("CacheRepository: loaded {N} records from {Path}.",
                        _cache.Count, _jsonPath);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "CacheRepository: could not load JSON file — starting fresh.");
            }
        }

        private static string? ExtractUnit(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            var parts = display.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[1] : null;
        }
    }
}
