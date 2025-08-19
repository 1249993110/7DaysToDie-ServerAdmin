using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    internal class ResponseCacheAttribute : ActionFilterAttribute
    {
        public int Duration { get; set; }

        public ResponseCacheAttribute()
        {
            Duration = 3600;
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response != null && context.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                context.Response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(Duration)
                };
            }

            base.OnActionExecuted(context);
        }
    }
}
