namespace EpiTrello.API.Middlewares;

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
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation("Request {Method} {Path} took {ElapsedMilliseconds} ms",
            context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
}