using LSTY.Sdtd.ServerAdmin.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Admins
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Admins")]
    [RequireGameStartDone]
    public class AdminsController : ApiController
    {
        
        [HttpPost]
        [Route("")]
        public IEnumerable<string> CreateAdmin([FromBody, MinLength(1)] AdminEntry admin)
        {
            string command = $"admin add {admin.PlayerId} {admin.PermissionLevel} {Utils.FormatCommandArgs(admin.DisplayName)}";
            return SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
        }

        /// <summary>
        /// Get admins
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IEnumerable<AdminEntry> GetAdmins()
        {
            var admins = new List<AdminEntry>();
            foreach (var item in GameManager.Instance.adminTools.Users.GetUsers().Values)
            {
                admins.Add(new AdminEntry()
                {
                    PlayerId = item.UserIdentifier.CombinedString,
                    PermissionLevel = item.PermissionLevel,
                    DisplayName = item.Name,
                });
            }

            return admins;
        }

        /// <summary>
        /// Remove admins
        /// </summary>
        /// <param name="playerIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public IEnumerable<string> RemoveAdmins([FromUri, MinLength(1)] string[] playerIds)
        {
            var executeResult = new List<string>();
            foreach (var item in playerIds)
            {
                string command = $"admin remove {item}";
                var result = SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
                executeResult.AddRange(result);
            }

            return executeResult;
        }
    }
}
