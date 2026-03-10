using Serilog.Context;
namespace FeatureTelemetry.SampleApi.Middleware;
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;
    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var value) ? value.ToString() : Guid.NewGuid().ToString("N");
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        using (LogContext.PushProperty("correlation_id", correlationId)) { await _next(context); }
    }
}