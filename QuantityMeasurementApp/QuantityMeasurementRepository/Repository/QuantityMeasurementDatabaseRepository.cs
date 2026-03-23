using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementModel.Interface;
using QuantityMeasurementRepository.Exception;

using System.Collections.Generic;
using System.Data;

namespace QuantityMeasurementRepository.Repository
{
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementEntityRepository
    {
        private readonly string _connectionString;

        public QuantityMeasurementDatabaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("QuantityMeasurementDB")
                ?? throw new DatabaseException("Connection string 'QuantityMeasurementDB' not found in appsettings.json.");
        }

        // ── Save ─────────────────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_SaveMeasurement", connection);
                command.CommandType = CommandType.StoredProcedure;

                var (firstVal, firstUnit, firstCat)    = ParseOperand(entity.FirstOperand);
                var (secondVal, secondUnit, secondCat) = ParseOperand(entity.SecondOperand);
                var (resultVal, resultUnit, _)         = ParseOperand(entity.Result);

                command.Parameters.AddWithValue("@MeasurementId",           Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@OperationType",            entity.OperationType);
                command.Parameters.AddWithValue("@FirstOperandValue",        (object?)firstVal    ?? DBNull.Value);
                command.Parameters.AddWithValue("@FirstOperandUnit",         (object?)firstUnit   ?? DBNull.Value);
                command.Parameters.AddWithValue("@FirstOperandCategory",     (object?)firstCat    ?? DBNull.Value);
                command.Parameters.AddWithValue("@FirstOperandDisplay",      (object?)entity.FirstOperand  ?? DBNull.Value);
                command.Parameters.AddWithValue("@SecondOperandValue",       (object?)secondVal   ?? DBNull.Value);
                command.Parameters.AddWithValue("@SecondOperandUnit",        (object?)secondUnit  ?? DBNull.Value);
                command.Parameters.AddWithValue("@SecondOperandCategory",    (object?)secondCat   ?? DBNull.Value);
                command.Parameters.AddWithValue("@SecondOperandDisplay",     (object?)entity.SecondOperand ?? DBNull.Value);
                command.Parameters.AddWithValue("@TargetUnit",               DBNull.Value);
                command.Parameters.AddWithValue("@ResultValue",              (object?)resultVal   ?? DBNull.Value);
                command.Parameters.AddWithValue("@ResultUnit",               (object?)resultUnit  ?? DBNull.Value);
                command.Parameters.AddWithValue("@FormattedResult",          (object?)entity.Result        ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsSuccessful",             !entity.IsError);
                command.Parameters.AddWithValue("@ErrorDetails",             (object?)entity.ErrorMessage  ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("sp_SaveMeasurement failed: " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // ── GetAllMeasurements ────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_GetAllMeasurements", connection);
                command.CommandType = CommandType.StoredProcedure;

                return ReadEntities(command);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("sp_GetAllMeasurements failed: " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // ── Clear ────────────────────────────────────────────────────────

        public void Clear()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_DeleteAllMeasurements", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("sp_DeleteAllMeasurements failed: " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // ── GetMeasurementsByOperationType ───────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByOperationType(string operationType)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_GetMeasurementsByOperationType", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@OperationType", operationType);

                return ReadEntities(command);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("sp_GetMeasurementsByOperationType failed: " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // ── GetMeasurementsByMeasurementType ─────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetMeasurementsByMeasurementType(string measurementType)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_GetMeasurementsByCategory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Category", measurementType);
                return ReadEntities(command);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("sp_GetMeasurementsByCategory failed: " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // ── GetTotalCount ────────────────────────────────────────────────

        public int GetTotalCount()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_GetTotalCount", connection);
                command.CommandType = CommandType.StoredProcedure;

                object? result = command.ExecuteScalar();
                return result == null ? 0 : Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("sp_GetTotalCount failed: " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private static IReadOnlyList<QuantityMeasurementEntity> ReadEntities(SqlCommand command)
        {
            List<QuantityMeasurementEntity> list = new List<QuantityMeasurementEntity>();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapRow(reader));
            }

            reader.Close();
            return list.AsReadOnly();
        }

        private static QuantityMeasurementEntity MapRow(SqlDataReader reader)
        {
            string  opType       = reader["OperationType"]?.ToString() ?? "";
            bool    isSuccessful = reader["IsSuccessful"] != DBNull.Value && (bool)reader["IsSuccessful"];
            string? errorDetail  = reader["ErrorDetails"] as string;

            // Read the stored display strings for operands and result
            string? firstDisplay  = reader["FirstOperandDisplay"]  as string;
            string? secondDisplay = reader["SecondOperandDisplay"] as string;
            string? formatted     = reader["FormattedResult"]      as string;

            if (!isSuccessful)
                return new QuantityMeasurementEntity(opType, (QuantityDTO?)null, (QuantityDTO?)null,
                    errorDetail ?? "", true);

            // Reconstruct using QuantityDTO built from display strings so entity.ToString() works
            QuantityDTO? firstDto  = BuildDto(firstDisplay);
            QuantityDTO? secondDto = BuildDto(secondDisplay);

            if (secondDto != null && firstDto != null)
                return new QuantityMeasurementEntity(opType, firstDto, secondDto, formatted ?? "");

            if (firstDto != null)
            {
                QuantityDTO resultDto = BuildDto(formatted) ?? new QuantityDTO(0, "", "");
                return new QuantityMeasurementEntity(opType, firstDto, resultDto);
            }

            return new QuantityMeasurementEntity(opType, (QuantityDTO?)null, (QuantityDTO?)null,
                formatted ?? "", false);
        }

        /// <summary>Builds a QuantityDTO from a display string like "1 FEET".</summary>
        private static QuantityDTO? BuildDto(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;

            var parts = display.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            double value = parts.Length > 0 && double.TryParse(parts[0], out var v) ? v : 0;
            string unit  = parts.Length > 1 ? parts[1] : "";
            string cat   = parts.Length > 2 ? parts[2] : "UNKNOWN";

            return new QuantityDTO(value, unit, cat);
        }

        private static (double? value, string? unit, string? category) ParseOperand(string? operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) return (null, null, null);

            var parts    = operand.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            double? value    = parts.Length > 0 && double.TryParse(parts[0], out var v) ? v : null;
            string? unit     = parts.Length > 1 ? parts[1] : null;
            string? category = parts.Length > 2 ? parts[2] : null;

            return (value, unit, category);
        }
    }
}