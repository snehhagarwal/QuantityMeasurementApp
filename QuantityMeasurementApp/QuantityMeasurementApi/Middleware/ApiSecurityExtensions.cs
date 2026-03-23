using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace QuantityMeasurementApi.Middleware
{
    /// <summary>Registers CORS, JWT Bearer validation, and Swagger with Bearer auth.</summary>
    public static class ApiSecurityExtensions
    {
        public const string AllowAllPolicy = "AllowAll";

        public static IServiceCollection AddApiCors(this IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy(AllowAllPolicy, policy =>
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));
            return services;
        }

        public static IServiceCollection AddApiJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            string secretKey = config["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey is missing from appsettings.json.");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = ctx =>
                        {
                            ctx.HandleResponse();
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            ctx.Response.ContentType = "application/json";
                            return ctx.Response.WriteAsync(
                                "{\"status\":401,\"error\":\"Unauthorized\"," +
                                "\"message\":\"JWT token is missing or invalid.\"}");
                        }
                    };
                });

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddApiSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Quantity Measurement API",
                    Version = "v1",
                    Description = "REST API - JWT protected.\n\n" +
                                  "1. POST /api/v1/users/signup to create an account.\n" +
                                  "2. POST /api/v1/users/login to get a token.\n" +
                                  "3. Click Authorize and paste ONLY the token (Swagger adds Bearer)."
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Paste your JWT (without the 'Bearer ' prefix).",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });

                var xml = Path.Combine(AppContext.BaseDirectory, "QuantityMeasurementApi.xml");
                if (File.Exists(xml)) c.IncludeXmlComments(xml);
            });
            return services;
        }
    }
}
