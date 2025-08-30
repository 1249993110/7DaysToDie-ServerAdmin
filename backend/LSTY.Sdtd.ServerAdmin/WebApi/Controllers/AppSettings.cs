using System.Web.Http;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/AppSettings")]
    public class AppSettingsController : ApiController
    {
    }
}
