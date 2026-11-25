using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.API.Middleware
{
    /// <summary>
    /// Factory-based middleware (implements <see cref="IMiddleware"/>, resolved from DI per request)
    /// that catches any exception the controllers did not handle and turns it into an
    /// RFC 7807 ProblemDetails response instead of a raw 500 / stack trace.
    ///
    /// - UnauthorizedAccessException -> 401
    /// - anything else               -> 500 with a generic message (details are logged, never sent)
    /// The response includes the correlation ID so a client error can be matched to the server log.
    /// </summary>
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // The client disconnected; there is nobody to send a response to.
                _logger.LogInformation("Request was cancelled by the client.");
            }
            catch (Exception exception)
            {
                if (context.Response.HasStarted)
                {
                    // Too late to change the status code or body; log and let the server abort the response.
                    _logger.LogError(exception, "Unhandled exception after the response had started.");
                    throw;
                }

                await WriteProblemDetailsAsync(context, exception);
            }
        }

        private async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, detail) = exception switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.",
                      _environment.IsDevelopment() ? exception.Message : "Please contact support with the correlation ID.")
            };

            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path.Value);
            }
            else
            {
                _logger.LogWarning("Request failed with {StatusCode}: {Message}", statusCode, exception.Message);
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
            problem.Extensions["correlationId"] = context.TraceIdentifier;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}
