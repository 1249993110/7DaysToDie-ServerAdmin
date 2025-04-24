using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.WebApi.OperationProcessors;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Entities;
using System.Security.Claims;

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
                string? userId = user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
                if(string.IsNullOrEmpty(userId) == false)
                {
                    if(context.Resource is DefaultHttpContext httpContext)
                    {
                        string? gameServerId;
                        if (httpContext.WebSockets.IsWebSocketRequest)
                        {
                            gameServerId = httpContext.WebSockets.WebSocketRequestedProtocols.ElementAtOrDefault(1);
                        }
                        else
                        {
                            gameServerId = httpContext.Request.Headers[AddGameServerIdHeaderParameter.Name].ToString();
                        }

                        if(string.IsNullOrEmpty(gameServerId) == false)
                        {
                            bool exists = await DB.Find<GameServerConfig>()
                                .Match(p => p.ID == gameServerId.ToString() && p.UserId == userId)
                                .ExecuteAnyAsync();

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
}
