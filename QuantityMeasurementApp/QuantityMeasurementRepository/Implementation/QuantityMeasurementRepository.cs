using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Context;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementRepository.Implementation
{
    public class QuantityMeasurementRepository : IQuantityMeasurementRepository
    {
        private readonly QuantityMeasurementDbContext _context;

        public QuantityMeasurementRepository(QuantityMeasurementDbContext context)
            => _context = context;

        public async Task<QuantityMeasurement> SaveAsync(QuantityMeasurement entity, CancellationToken cancellationToken = default)
        {
            _context.Measurements.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<IReadOnlyList<QuantityMeasurement>> FindAllAsync(CancellationToken cancellationToken = default)
            => await _context.Measurements
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByOperationTypeAsync(string operationType, CancellationToken cancellationToken = default)
        {
            var upper = operationType.ToUpperInvariant();
            return await _context.Measurements
                .AsNoTracking()
                .Where(e => e.OperationType == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByMeasurementTypeAsync(string measurementType, CancellationToken cancellationToken = default)
        {
            var upper = measurementType.ToUpperInvariant();
            return await _context.Measurements
                .AsNoTracking()
                .Where(e => e.FirstOperandCategory != null && e.FirstOperandCategory == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByCreatedAtAfterAsync(DateTime after, CancellationToken cancellationToken = default)
            => await _context.Measurements
                .AsNoTracking()
                .Where(e => e.CreatedAt > after)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<QuantityMeasurement>> FindByIsErrorTrueAsync(CancellationToken cancellationToken = default)
            => await _context.Measurements
                .AsNoTracking()
                .Where(e => !e.IsSuccessful)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);

        public async Task<long> CountByOperationTypeAndIsErrorFalseAsync(string operationType, CancellationToken cancellationToken = default)
        {
            var upper = operationType.ToUpperInvariant();
            return await _context.Measurements
                .LongCountAsync(e => e.OperationType == upper && e.IsSuccessful, cancellationToken);
        }

        public async Task<long> CountAsync(CancellationToken cancellationToken = default)
            => await _context.Measurements.LongCountAsync(cancellationToken);
    }
}
