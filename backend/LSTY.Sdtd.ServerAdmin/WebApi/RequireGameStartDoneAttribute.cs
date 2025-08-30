using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    internal class RequireGameStartDoneAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (ModMain.IsGameStartDone)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The game server is still initializing.");
        }
    }
}
