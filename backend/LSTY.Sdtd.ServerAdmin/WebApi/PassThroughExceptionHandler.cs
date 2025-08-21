using System.Web.Http.ExceptionHandling;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    /// <summary>
    /// A "pass-through" exception handler.
    /// Instead of handling the exception, it rethrows it so that it can be caught by a higher-level handler (such as an OWIN middleware or ASP.NET YSOD).
    /// </summary>
    internal class PassThroughExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            throw context.Exception; // Rethrow the exception to propagate it up the pipeline.
        }
    }
}
