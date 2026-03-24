using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementModel.Context;

namespace QuantityMeasurementRepository.Context
{
    /// <summary>Design-time factory for <c>dotnet ef migrations</c> (Repository as migrations assembly).</summary>
    public class QuantityMeasurementDbContextFactory : IDesignTimeDbContextFactory<QuantityMeasurementDbContext>
    {
        public QuantityMeasurementDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../QuantityMeasurementApi"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<QuantityMeasurementDbContext>();
            optionsBuilder.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(QuantityMeasurementDbContextFactory).Assembly.GetName().Name));

            return new QuantityMeasurementDbContext(optionsBuilder.Options);
        }
    }
}