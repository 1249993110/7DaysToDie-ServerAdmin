using Microsoft.AspNetCore.Authorization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authorization
{
    public class GameServerOwnerRequirement : IAuthorizationRequirement
    {
        public GameServerOwnerRequirement()
        {
        }
    }
}
