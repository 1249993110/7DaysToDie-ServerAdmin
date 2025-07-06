using Microsoft.AspNetCore.Authorization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Authentication.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Gets the login status of the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetLoginStatus()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok();
            }

            return Unauthorized();
        }
    }
}
