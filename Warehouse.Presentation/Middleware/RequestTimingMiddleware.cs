using System.Diagnostics;

namespace Warehouse.Presentation.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Response-Time-ms"] = stopwatch.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            LogSlowRequest(context, stopwatch.ElapsedMilliseconds);
        }
    }

    private void LogSlowRequest(HttpContext context, long executionTime)
    {
        if (executionTime <= 500)
            return;

        _logger.LogWarning(
            "Slow request: {Method} {Endpoint} returned {StatusCode} and took {ExecutionTime} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            executionTime);
    }
}