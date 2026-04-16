using System.Text.Json;

namespace QmaService.Middleware;

public sealed class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Path}", ctx.Request.Path);
            if (ctx.Response.HasStarted) throw;
            (int status, string error) = ex switch
            {
                DivideByZeroException => (400, "Bad Request"),
                InvalidOperationException => (400, "Bad Request"),
                UnauthorizedAccessException => (401, "Unauthorized"),
                _ => (500, "Internal Server Error")
            };
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
            { timestamp = DateTime.UtcNow, status, error, message = ex.Message, path = ctx.Request.Path.Value }));
        }
    }
}
