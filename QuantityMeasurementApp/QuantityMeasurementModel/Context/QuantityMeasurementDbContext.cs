using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementModel.Context
{
    /// <summary>EF Core DbContext — quantity_measurements and users tables.</summary>
    public class QuantityMeasurementDbContext : DbContext
    {
        public QuantityMeasurementDbContext(
            DbContextOptions<QuantityMeasurementDbContext> options) : base(options) { }

        public DbSet<QuantityMeasurement> Measurements { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<QuantityMeasurement>(e =>
            {
                e.ToTable("quantity_measurements");
                e.HasIndex(m => m.OperationType).HasDatabaseName("idx_operation_type");
                e.HasIndex(m => m.FirstOperandCategory).HasDatabaseName("idx_measurement_type");
                e.HasIndex(m => m.CreatedAt).HasDatabaseName("idx_created_at");
                e.HasIndex(m => m.IsSuccessful).HasDatabaseName("idx_is_successful");
                e.HasIndex(m => m.UserId).HasDatabaseName("idx_user_id");
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasIndex(u => u.Username).IsUnique().HasDatabaseName("idx_users_username");
                e.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");
            });
        }
    }
}
