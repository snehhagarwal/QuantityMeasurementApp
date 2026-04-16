using Microsoft.EntityFrameworkCore;
using QmaService.Entities;

namespace QmaService.Data;

public class QmaDbContext(DbContextOptions<QmaDbContext> options) : DbContext(options)
{
    public DbSet<QuantityMeasurement> Measurements => Set<QuantityMeasurement>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<QuantityMeasurement>(e =>
        {
            e.HasIndex(m => m.UserId);
            e.HasIndex(m => m.OperationType);
            e.HasIndex(m => m.IsSuccessful);
        });
    }
}
