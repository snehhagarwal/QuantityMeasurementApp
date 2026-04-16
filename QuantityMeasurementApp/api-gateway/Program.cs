using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── YARP Reverse Proxy ────────────────────────────────────────────────────
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ── JWT validation ────────────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwt["Issuer"],
            ValidAudience            = jwt["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwt["SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode  = 401;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync(
                    "{\"status\":401,\"error\":\"Unauthorized\",\"message\":\"JWT token is missing or invalid.\"}");
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient("swagger");

// ── Swagger: two definitions (one per service) ────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("auth", new OpenApiInfo
    {
        Title       = "Auth Service — via Gateway",
        Version     = "v1",
        Description = "User signup, login and profile endpoints proxied through port 5000"
    });
    c.SwaggerDoc("qma", new OpenApiInfo
    {
        Title       = "QMA Service — via Gateway",
        Version     = "v1",
        Description = "Quantity measurement endpoints proxied through port 5000"
    });

    var scheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Reference    = new OpenApiReference
        {
            Id   = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});

var app = builder.Build();

app.UseMiddleware<GatewayLoggingMiddleware>();

// ── Swagger UI ────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger-proxy/auth", "Auth Service");
    c.SwaggerEndpoint("/swagger-proxy/qma",  "QMA Service");
    c.RoutePrefix = "swagger";
});

// ── Aggregation proxy: fetches swagger.json from each downstream service
//    and rewrites the "servers" so Try-it-out calls go through port 5000 ──
app.MapGet("/swagger-proxy/{service}", async (
    string service,
    IHttpClientFactory factory,
    IConfiguration config) =>
{
    var address = service switch
    {
        "auth" => config["ReverseProxy:Clusters:auth-cluster:Destinations:auth-service:Address"]
                  ?? "http://auth-service:8080",
        "qma"  => config["ReverseProxy:Clusters:qma-cluster:Destinations:qma-service:Address"]
                  ?? "http://qma-service:8080",
        _      => null
    };

    if (address is null)
        return Results.NotFound(new { error = $"Unknown service: {service}" });

    var client = factory.CreateClient("swagger");
    try
    {
        var json = await client.GetStringAsync($"{address}/swagger/v1/swagger.json");

        // Rewrite servers array with System.Text.Json — no extra packages needed
        var doc = JsonNode.Parse(json)!.AsObject();
        doc["servers"] = new JsonArray(
            new JsonObject { ["url"] = "/", ["description"] = "API Gateway (port 5000)" }
        );

        return Results.Content(
            doc.ToJsonString(new JsonSerializerOptions { WriteIndented = false }),
            "application/json");
    }
    catch (HttpRequestException)
    {
        return Results.Problem(
            detail:     $"Could not reach '{service}' service at {address}. Is it running?",
            statusCode: 502,
            title:      "Service Unavailable");
    }
})
.ExcludeFromDescription();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapReverseProxy();
app.MapHealthChecks("/actuator/health");

await app.RunAsync();