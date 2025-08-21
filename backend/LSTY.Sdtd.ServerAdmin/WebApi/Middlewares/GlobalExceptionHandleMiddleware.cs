using Microsoft.Owin;
using Newtonsoft.Json;
using System.Net;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Middlewares
{
    /// <summary>
    /// Global middleware for handling exceptions across the application.
    /// Catches unhandled exceptions and returns a standardized JSON response.
    /// </summary>
    public class GlobalExceptionHandleMiddleware : OwinMiddleware
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandleMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        public GlobalExceptionHandleMiddleware(OwinMiddleware next, JsonSerializerSettings jsonSerializerSettings) : base(next)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {

                string traceId;

                // Try to get the TraceId from the request header (common header names are X-Request-ID or X-Correlation-ID)
                // This allows upstream services (such as API Gateway) to pass in existing TraceIds
                string? traceIdHeader = context.Request.Headers["X-Request-ID"]?.ToString();
                if (traceIdHeader == null)
                {
                    traceId = Guid.NewGuid().ToString("N");
                }
                else
                {
                    traceId = traceIdHeader;
                }

                // Add TraceId to the response header
                context.Response.Headers.Append("X-Request-ID", traceId);

                var response = context.Response;

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(new
                {
                    TraceId = traceId,
                    Message = "Internal server error.",
                    ExceptionMessage = ex.Message,
                    ExceptionType = ex.GetType().FullName,
                    StackTrace = ex.StackTrace
                }, _jsonSerializerSettings);
                await response.WriteAsync(json);

                CustomLogger.Error(
                    ex,
                    "The exception is caught in the global exception handling middleware, traceId: {0}, path: {1}",
                    traceId,
                    context.Request.Path);
            }
        }
    }
}