using System.Text.Json;

namespace AuthService.Middleware
{
    public sealed class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        { _next = next; _logger = logger; }

        public async Task InvokeAsync(HttpContext ctx)
        {
            try { await _next(ctx); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Path}", ctx.Request.Path);
                if (ctx.Response.HasStarted) throw;
                (int status, string error) = ex switch
                {
                    UnauthorizedAccessException => (401, "Unauthorized"),
                    ArgumentException => (409, "Conflict"),
                    _ => (500, "Internal Server Error")
                };
                ctx.Response.StatusCode = status;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
                { timestamp = DateTime.UtcNow, status, error, message = ex.Message, path = ctx.Request.Path.Value }));
            }
        }
    }
}
