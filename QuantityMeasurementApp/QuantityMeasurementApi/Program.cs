using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Context;
using QuantityMeasurementApi.Middleware;
using QuantityMeasurementBusinessLayer.Services.Implementation;
using QuantityMeasurementBusinessLayer.Services.Interface;
using QuantityMeasurementRepository.Interface;
using QuantityMeasurementRepository.Repository;
using QuantityMeasurementModel.Interface;
using EfQuantityRepository = QuantityMeasurementRepository.Service.QuantityMeasurementRepository;
using EfUserRepository     = QuantityMeasurementRepository.Service.UserRepository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── Determine repository type from configuration ───────────────────────────
var repoType = builder.Configuration["RepositoryType"] ?? "Cache";
var useRedis  = repoType.Equals("Redis", StringComparison.OrdinalIgnoreCase);

// ── Database (SQL Server via EF Core) — required only for Redis mode ────────
if (useRedis)
{
    builder.Services.AddDbContext<QuantityMeasurementDbContext>((sp, options) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var useDb  = config["UseDatabase"] ?? "SqlServer";

        if (useDb.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            string dbName = config["InMemoryDatabaseName"] ?? "quantity_measurement_inmemory";
            options.UseInMemoryDatabase(dbName);
        }
        else
        {
            // Default: SQL Server stored in SSMS
           var useDb = config["UseDatabase"] ?? "SqlServer";
if (useDb.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    options.UseNpgsql(
        config.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("QuantityMeasurementRepository"));
else
    options.UseSqlServer(
        config.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("QuantityMeasurementRepository"));
        }
    });

    // ── Redis PRIMARY + SQL Server dual-write ──────────────────────────────
    var redisConn = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        _ => ConnectionMultiplexer.Connect(redisConn));

    // Repository: Redis reads → SQL Server durable writes (stored in SSMS)
    builder.Services.AddScoped<IQuantityMeasurementEntityRepository, RedisRepository>();
    builder.Services.AddScoped<IQuantityMeasurementRepository, EfQuantityRepository>();
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();

    // Service-layer cache: Redis
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();

    Console.WriteLine("[Repository] Redis PRIMARY + SQL Server (SSMS) dual-write selected.");
}
else
{
    // ── Cache Repository: In-Memory + JSON file ────────────────────────────
    // No SQL Server, no ADO.NET, no EF Core required in this mode.

    // Minimal DbContext for auth (users table) — uses SQL Server if configured,
    // otherwise falls back to in-memory so the app starts without any DB.
    builder.Services.AddDbContext<QuantityMeasurementDbContext>((sp, options) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var connStr = config.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connStr))
{
    var useDb = config["UseDatabase"] ?? "SqlServer";
    if (useDb.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        options.UseNpgsql(connStr,
            b => b.MigrationsAssembly("QuantityMeasurementRepository"));
    else
        options.UseSqlServer(connStr,
            b => b.MigrationsAssembly("QuantityMeasurementRepository"));
}
    });

    builder.Services.AddScoped<IUserRepository, EfUserRepository>();

    // In-Memory + JSON file repository for quantity measurements
    builder.Services.AddSingleton<IQuantityMeasurementEntityRepository, CacheRepository>();
    builder.Services.AddScoped<IQuantityMeasurementRepository, EfQuantityRepository>();

    // Service-layer cache: in-process MemoryCache
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

    Console.WriteLine("[Repository] Cache Repository (In-Memory + JSON file) selected.");
}

// ── Shared services ────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IAesEncryptionService, AesEncryptionService>();

builder.Services.AddApiCors();

if (string.Equals(builder.Configuration["DisableJwtAuth"], "true", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddAuthorization();
else
    builder.Services.AddApiJwtAuthentication(builder.Configuration);

builder.Services.AddApiSwagger();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Run EF migrations for ALL SQL Server modes (Redis and Cache)
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<QuantityMeasurementDbContext>();
    if (db.Database.IsRelational())
    {
        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
        c.RoutePrefix = "swagger-ui";
    });
}

app.UseCors(ApiSecurityExtensions.AllowAllPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/actuator/health");

await app.RunAsync();

public partial class Program { }