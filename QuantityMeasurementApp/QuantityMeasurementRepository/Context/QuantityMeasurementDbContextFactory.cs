using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QuantityMeasurementModel.Context;

namespace QuantityMeasurementRepository.Context
{
    /// <summary>Design-time factory for <c>dotnet ef migrations</c> (Repository as migrations assembly).</summary>
    public class QuantityMeasurementDbContextFactory : IDesignTimeDbContextFactory<QuantityMeasurementDbContext>
    {
        public QuantityMeasurementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<QuantityMeasurementDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=QuantityMeasurementDB;Trusted_Connection=True;TrustServerCertificate=True;",
                b => b.MigrationsAssembly(typeof(QuantityMeasurementDbContextFactory).Assembly.GetName().Name));
            return new QuantityMeasurementDbContext(optionsBuilder.Options);
        }
    }
}
