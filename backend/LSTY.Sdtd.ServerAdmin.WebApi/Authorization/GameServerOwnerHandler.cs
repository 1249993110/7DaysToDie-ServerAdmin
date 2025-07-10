using IceCoffee.Common.Extensions;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.WebApi.Extensions;
using LSTY.Sdtd.ServerAdmin.WebApi.OperationProcessors;
using Microsoft.AspNetCore.Authorization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authorization
{
    /// <summary>
    /// Authorization handler for game server owner.
    /// </summary>
    public class GameServerOwnerHandler : AuthorizationHandler<GameServerOwnerRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerOwnerHandler"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GameServerOwnerRequirement requirement)
        {
            var user = context.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                string userId = user.GetUserId();
                if (context.Resource is DefaultHttpContext httpContext)
                {
                    string? gameServerIdStr;
                    if (httpContext.WebSockets.IsWebSocketRequest)
                    {
                        gameServerIdStr = httpContext.WebSockets.WebSocketRequestedProtocols.ElementAtOrDefault(1);
                    }
                    else
                    {
                        gameServerIdStr = httpContext.Request.Headers[AddGameServerIdHeaderParameter.Name].ToString();
                    }

                    if (string.IsNullOrEmpty(gameServerIdStr) == false && Guid.TryParse(gameServerIdStr, out Guid gameServerId))
                    {
                        bool exists = await Db.QueryExists<GameServerConfig>()
                            .WhereEq(p => p.Id, gameServerId)
                            .WhereEq(p => p.UserId, userId)
                            .GetAsync();

                        if (exists)
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }
    }
}
