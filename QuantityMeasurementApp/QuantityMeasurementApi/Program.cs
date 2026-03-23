using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Context;
using QuantityMeasurementApi.Middleware;
using QuantityMeasurementBusinessLayer.Services.Implementation;
using QuantityMeasurementBusinessLayer.Services.Interface;
using QuantityMeasurementRepository.Interface;
using EfQuantityRepository = QuantityMeasurementRepository.Implementation.QuantityMeasurementRepository;
using EfUserRepository = QuantityMeasurementRepository.Implementation.UserRepository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Single DbContext registration — provider chosen when the context is configured (after test config is merged).
builder.Services.AddDbContext<QuantityMeasurementDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var useDb = config["UseDatabase"] ?? "Sqlite";
    if (useDb.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
    {
        string dbName = config["InMemoryDatabaseName"] ?? "quantity_measurement_inmemory";
        options.UseInMemoryDatabase(dbName);
    }
    else if (useDb.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(
            config.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("QuantityMeasurementRepository"));
    }
    else
    {
        options.UseSqlite(
            "Data Source=quantity_measurement.db",
            b => b.MigrationsAssembly("QuantityMeasurementRepository"));
    }
});

builder.Services.AddScoped<IQuantityMeasurementRepository, EfQuantityRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IAesEncryptionService, AesEncryptionService>();

builder.Services.AddMemoryCache();
var redisEnabled = builder.Configuration.GetValue("Redis:Enabled", false);
var redisConn = builder.Configuration["Redis:ConnectionString"];
if (redisEnabled && !string.IsNullOrWhiteSpace(redisConn))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
}

builder.Services.AddApiCors();

if (string.Equals(builder.Configuration["DisableJwtAuth"], "true", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddAuthorization();
else
    builder.Services.AddApiJwtAuthentication(builder.Configuration);

builder.Services.AddApiSwagger();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

var useDbForInit = app.Configuration["UseDatabase"] ?? "Sqlite";
if (!useDbForInit.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
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
    else
        await db.Database.EnsureCreatedAsync();
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
