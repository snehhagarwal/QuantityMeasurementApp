using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using QmaService.Data;
using QmaService.Interfaces;
using QmaService.Middleware;
using QmaService.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<QmaDbContext>((sp, opt) =>
{
    var cfg  = sp.GetRequiredService<IConfiguration>();
    var conn = cfg.GetConnectionString("DefaultConnection");
    var db   = cfg["UseDatabase"] ?? "PostgreSQL";
    if (db.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        opt.UseInMemoryDatabase("qma_service_inmemory");
    else if (db.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        opt.UseNpgsql(conn, b => b.MigrationsAssembly("QmaService"));
    else
        opt.UseNpgsql(conn, b => b.MigrationsAssembly("QmaService"));
});

// ── Redis Cache ───────────────────────────────────────────────────────────
var redisConn = builder.Configuration["Redis:ConnectionString"] ?? "redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// ── Business Services ─────────────────────────────────────────────────────
builder.Services.AddScoped<IQmaService, QmaMeasurementService>();
builder.Services.AddHttpContextAccessor();

// ── JWT Auth (validates tokens issued by auth-service) ────────────────────
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!)),
            ValidateIssuer   = true, ValidIssuer   = jwt["Issuer"],
            ValidateAudience = true, ValidAudience = jwt["Audience"],
            ValidateLifetime = true, ClockSkew     = TimeSpan.Zero
        };
        o.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode  = 401;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync("{\"status\":401,\"error\":\"Unauthorized\",\"message\":\"JWT token is missing or invalid.\"}");
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QMA Service", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer",
        BearerFormat = "JWT", In = ParameterLocation.Header,
        Reference = new OpenApiReference { Id = JwtBearerDefaults.AuthenticationScheme, Type = ReferenceType.SecurityScheme }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// ── Migrations ────────────────────────────────────────────────────────────
await using (var scope = app.Services.CreateAsyncScope())
{
    var qmaDb = scope.ServiceProvider.GetRequiredService<QmaDbContext>();
    if (qmaDb.Database.IsRelational()) await qmaDb.Database.EnsureCreatedAsync();
    else await qmaDb.Database.EnsureCreatedAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "QMA Service v1"); c.RoutePrefix = "swagger"; });
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/actuator/health");

await app.RunAsync();
