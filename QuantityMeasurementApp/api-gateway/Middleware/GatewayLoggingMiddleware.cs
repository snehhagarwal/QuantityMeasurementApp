namespace ApiGateway.Middleware;

public sealed class GatewayLoggingMiddleware(RequestDelegate next, ILogger<GatewayLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("[Gateway] {Method} {Path}", context.Request.Method, context.Request.Path);
        await next(context);
        logger.LogInformation("[Gateway] {Method} {Path} → {Status}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
    }
}
