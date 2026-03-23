using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Interface;

namespace QuantityMeasurementRepository.Repository
{
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementEntityRepository
    {
        private static readonly QuantityMeasurementCacheRepository _instance
            = new QuantityMeasurementCacheRepository();

        public static QuantityMeasurementCacheRepository Instance => _instance;

        private readonly List<QuantityMeasurementEntity> _cache = new();
        private static readonly string JsonFilePath = GetJsonFilePath();

        private static string GetJsonFilePath()
        {
            // bin/Debug/net10.0/ → go up 3 levels to reach the project folder
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(baseDir, "..", "..", ".."));
            return System.IO.Path.Combine(projectDir, "quantity_measurements.json");
        }

        private QuantityMeasurementCacheRepository() { LoadFromJson(); }

        public void Save(QuantityMeasurementEntity entity)
        {
            _cache.Add(entity);
            SaveAllToJson();
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
            => _cache.AsReadOnly();

        public void Clear()
        {
            _cache.Clear();
            if (File.Exists(JsonFilePath)) File.Delete(JsonFilePath);
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType)
            => _cache.Where(e => string.Equals(e.OperationType, operationType,
                StringComparison.OrdinalIgnoreCase)).ToList().AsReadOnly();

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType)
            => _cache.Where(e =>
                (e.FirstOperand  != null && e.FirstOperand.Contains(measurementType,  StringComparison.OrdinalIgnoreCase)) ||
                (e.SecondOperand != null && e.SecondOperand.Contains(measurementType, StringComparison.OrdinalIgnoreCase)) ||
                (e.Result        != null && e.Result.Contains(measurementType,        StringComparison.OrdinalIgnoreCase)))
                .ToList().AsReadOnly();

        public int GetTotalCount() => _cache.Count;

        // ── JSON persistence ───────────────────────────────────────────────

        private void SaveAllToJson()
        {
            try
            {
                var records = _cache.Select(e => new JsonRecord
                {
                    OperationType = e.OperationType,
                    FirstOperand  = e.FirstOperand,
                    SecondOperand = e.SecondOperand,
                    Result        = e.Result,
                    IsError       = e.IsError,
                    ErrorMessage  = e.ErrorMessage,
                    Timestamp     = e.Timestamp.ToString("o")
                }).ToList();

                string json = JsonSerializer.Serialize(records,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(JsonFilePath, json);
            }
            catch (System.Exception) { }
        }

        private void LoadFromJson()
        {
            if (!File.Exists(JsonFilePath)) return;
            try
            {
                string json = File.ReadAllText(JsonFilePath);
                var records  = JsonSerializer.Deserialize<List<JsonRecord>>(json);
                if (records == null) return;

                foreach (var r in records)
                {
                    QuantityMeasurementEntity entity;

                    if (r.IsError)
                    {
                        entity = new QuantityMeasurementEntity(
                            r.OperationType ?? "HISTORY",
                            (QuantityDTO?)null,
                            (QuantityDTO?)null,
                            r.ErrorMessage ?? "", true);
                    }
                    else if (!string.IsNullOrWhiteSpace(r.SecondOperand))
                    {
                        // Binary operation — use the constructor that sets both operands
                        entity = new QuantityMeasurementEntity(
                            r.OperationType ?? "HISTORY",
                            BuildDtoFromDisplay(r.FirstOperand),
                            BuildDtoFromDisplay(r.SecondOperand),
                            r.Result ?? "");
                    }
                    else
                    {
                        // Single operand (e.g. CONVERT)
                        var first  = BuildDtoFromDisplay(r.FirstOperand);
                        var result = BuildDtoFromDisplay(r.Result);
                        if (first != null && result != null)
                            entity = new QuantityMeasurementEntity(r.OperationType ?? "HISTORY", first, result);
                        else
                            entity = new QuantityMeasurementEntity(
                                r.OperationType ?? "HISTORY",
                                (QuantityDTO?)null, (QuantityDTO?)null,
                                r.Result ?? "", false);
                    }

                    _cache.Add(entity);
                }
            }
            catch (System.Exception) { }
        }

        private static QuantityDTO? BuildDtoFromDisplay(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            var parts = display.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            double value = parts.Length > 0 && double.TryParse(parts[0], out var v) ? v : 0;
            string unit  = parts.Length > 1 ? parts[1] : "";
            return new QuantityDTO(value, unit, "UNKNOWN");
        }

        private class JsonRecord
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