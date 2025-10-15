using LSTY.Sdtd.ServerAdmin.Config;
using System.Web.Http;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// App Settings
    /// </summary>
    [Authorize]
    [RoutePrefix("api/AppSettings")]
    public class AppSettingsController : ApiController
    {
        /// <summary>
        /// Retrieves the current application settings.
        /// </summary>
        /// <remarks>The returned settings reflect the application's current configuration state. Changes
        /// to the configuration may not be reflected until the application is restarted, depending on how settings are
        /// managed.</remarks>
        /// <returns>An <see cref="AppSettings"/> instance containing the application's configuration values.</returns>
        [HttpGet]
        [Route("")]
        public AppSettings Get()
        {
            return AppConfig.Settings;
        }

        /// <summary>
        /// Updates the application settings with the values provided in the request body.
        /// </summary>
        /// <remarks>This method replaces the current application settings with those specified in
        /// <paramref name="appSettings"/>. The update is performed immediately and affects subsequent requests. Ensure
        /// that the provided settings are valid, as invalid values may impact application behavior.</remarks>
        /// <param name="appSettings">An <see cref="AppSettings"/> object containing the new configuration values to apply. Cannot be null.</param>
        /// <returns>An <see cref="IHttpActionResult"/> indicating the result of the update operation.</returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult Update([FromBody] AppSettings appSettings)
        {
            AppConfig.SaveSettings(appSettings);
            return Ok();
        }
    }
}
