using System.Net;
using System.Text.Json;
using QuantityMeasurementBusinessLayer.Exceptions;

namespace QuantityMeasurementApi.Middleware
{
    /// <summary>Maps unhandled exceptions to a consistent JSON error body and status code.</summary>
    public sealed class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
                await WriteErrorAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, Exception ex)
        {
            if (context.Response.HasStarted) throw ex;

            int status;
            string error;

            switch (ex)
            {
                case QuantityMeasurementException:
                    status = StatusCodes.Status400BadRequest;
                    error = "Quantity Measurement Error";
                    break;
                case UnauthorizedAccessException:
                    status = StatusCodes.Status401Unauthorized;
                    error = "Unauthorized";
                    break;
                case ArgumentException:
                    status = StatusCodes.Status409Conflict;
                    error = "Conflict";
                    break;
                case ArithmeticException:
                    status = StatusCodes.Status500InternalServerError;
                    error = "Internal Server Error";
                    break;
                default:
                    status = StatusCodes.Status500InternalServerError;
                    error = "Internal Server Error";
                    break;
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";

            var body = new
            {
                timestamp = DateTime.UtcNow,
                status,
                error,
                message = ex.Message,
                path = context.Request.Path.Value
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(body)).ConfigureAwait(false);
        }
    }
}
