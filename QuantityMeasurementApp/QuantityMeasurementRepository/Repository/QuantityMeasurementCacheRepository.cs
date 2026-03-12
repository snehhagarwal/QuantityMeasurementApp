using System;
using System.Collections.Generic;
using System.IO;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementRepository.Repository
{
    /// <summary>
    /// UC15: Singleton cache repository implementing IQuantityMeasurementRepository.
    ///
    /// Design:
    ///   - Singleton: one instance throughout the application (thread-safe via static init).
    ///   - In-memory ArrayList (List&lt;T&gt;) for fast session-level access.
    ///   - Disk persistence via AppendableObjectOutputStream pattern:
    ///     each Save() appends one text record without rewriting the file header.
    ///   - LoadFromDisk() on construction restores history across restarts.
    ///
    /// The AppendableObjectOutputStream concept from Java is implemented here as
    /// StreamWriter in append mode — each entity is persisted as one text line.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        // ── Singleton ──────────────────────────────────────────────────────────

        private static readonly QuantityMeasurementCacheRepository _instance
            = new QuantityMeasurementCacheRepository();

        /// <summary>Global access point to the single repository instance.</summary>
        public static QuantityMeasurementCacheRepository Instance => _instance;

        // ── In-memory cache (ArrayList equivalent) ─────────────────────────────

        private readonly List<QuantityMeasurementEntity> _cache = new();

        private static readonly string FilePath = "quantity_measurements.dat";

        // ── Private constructor — enforces Singleton ───────────────────────────

        private QuantityMeasurementCacheRepository()
        {
            LoadFromDisk();
        }

        // ── IQuantityMeasurementRepository ────────────────────────────────────

        /// <summary>
        /// Saves entity to in-memory cache AND appends to disk (AppendableObjectOutputStream pattern).
        /// </summary>
        public void Save(QuantityMeasurementEntity entity)
        {
            _cache.Add(entity);
            AppendToDisk(entity);
        }

        /// <summary>Returns read-only view of all cached entities.</summary>
        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
            => _cache.AsReadOnly();

        /// <summary>Clears in-memory cache and deletes the persistence file.</summary>
        public void Clear()
        {
            _cache.Clear();
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        // ── Disk persistence ───────────────────────────────────────────────────

        /// <summary>
        /// Appends a single entity record to the persistence file.
        /// Mirrors the Java AppendableObjectOutputStream: opens in append mode
        /// so existing records are never overwritten.
        /// </summary>
        private static void AppendToDisk(QuantityMeasurementEntity entity)
        {
            try
            {
                // append: true  →  equivalent to AppendableObjectOutputStream
                using var writer = new StreamWriter(FilePath, append: true);
                writer.WriteLine(entity.ToString());
            }
            catch (Exception)
            {
                // Disk errors must not crash the app — in-memory cache stays valid
            }
        }

        /// <summary>
        /// Loads persisted text records from disk into the in-memory cache on startup.
        /// Called once from the private constructor.
        /// </summary>
        private void LoadFromDisk()
        {
            if (!File.Exists(FilePath)) return;

            try
            {
                var lines = File.ReadAllLines(FilePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Reconstruct a display-only entity from the persisted line.
                    // Full deserialization would need JSON; text is sufficient for history.
                    var loaded = new QuantityMeasurementEntity(
                        "HISTORY",
                        (QuantityDTO?)null,
                        (QuantityDTO?)null,
                        line,
                        false
                    );
                    _cache.Add(loaded);
                }
            }
            catch (Exception)
            {
                // Corrupt file — start with empty cache
            }
        }
    }
}
