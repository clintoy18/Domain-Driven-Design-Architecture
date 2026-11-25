namespace DomainDrivenDesign.API.Middleware
{
    public static class MiddlewareExtensions
    {
        /// <summary>Registers services needed by the custom middleware (IMiddleware types live in DI).</summary>
        public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionMiddleware>();
            return services;
        }

        /// <summary>
        /// Adds the custom middleware in the right order: the correlation ID must exist before
        /// the exception handler runs, so error responses and logs can include it.
        /// Call this first in the pipeline so it wraps everything else.
        /// </summary>
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            return app;
        }
    }
}
