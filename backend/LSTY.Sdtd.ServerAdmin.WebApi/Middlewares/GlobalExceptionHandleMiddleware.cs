using LSTY.Sdtd.ServerAdmin.WebApi.Models;
using System.Diagnostics;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Middlewares
{
    /// <summary>
    /// Global middleware for handling exceptions across the application.
    /// Catches unhandled exceptions and returns a standardized JSON response.
    /// </summary>
    public class GlobalExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandleMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandleMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">The logger used for recording exception details.</param>
        public GlobalExceptionHandleMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP request.
        /// Catches any unhandled exceptions, logs them, and returns a standardized error response.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="jsonOptions">Options for JSON serialization.</param>
        /// <param name="env">The web hosting environment information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [DebuggerStepThrough]
        public async Task Invoke(HttpContext context, IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> jsonOptions, IWebHostEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = StatusCodes.Status500InternalServerError;

                string requestId = context.TraceIdentifier;
                var model = new InternalServerError()
                {
                    TraceId = requestId,
                    Message = env.IsDevelopment() ? ex.Message : "Internal server error",
                };

                await JsonSerializer.SerializeAsync(response.Body, model, jsonOptions.Value.JsonSerializerOptions);

                var path = context.Request.Path;

                _logger.LogError(
                    ex,
                    "The exception is caught in the global exception handling middleware, requestId: {requestId}, path: {path}",
                    requestId,
                    path);
            }
        }
    }
}