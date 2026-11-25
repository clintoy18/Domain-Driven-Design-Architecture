using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DomainDrivenDesign.API.Middleware
{
    /// <summary>
    /// Convention-based middleware that gives every request a correlation ID and logs one
    /// summary line per request (method, path, status code, duration).
    ///
    /// - Reuses the caller's X-Correlation-ID header when it is safe, otherwise generates one.
    /// - Echoes the ID back in the response header so clients can quote it in bug reports.
    /// - Opens a logging scope, so every log line written further down the pipeline
    ///   (controllers, services, the exception middleware) carries the same CorrelationId.
    /// </summary>
    public partial class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName].ToString());

            // Make the ID available to the rest of the pipeline (exception middleware, controllers).
            context.TraceIdentifier = correlationId;

            // Headers must be set before the response starts streaming, so register a callback.
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                        "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
            }
        }

        // Only trust short, plain IDs from callers; anything else could pollute logs (log injection).
        private static string ResolveCorrelationId(string incoming) =>
            !string.IsNullOrEmpty(incoming) && SafeIdPattern().IsMatch(incoming)
                ? incoming
                : Guid.NewGuid().ToString("N");

        [GeneratedRegex("^[A-Za-z0-9._-]{1,64}$")]
        private static partial Regex SafeIdPattern();
    }
}
