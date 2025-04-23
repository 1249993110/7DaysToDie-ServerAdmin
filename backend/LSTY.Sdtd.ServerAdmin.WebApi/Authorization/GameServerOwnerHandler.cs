using LSTY.Sdtd.ServerAdmin.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Entities;
using System.Security.Claims;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authorization
{
    public class GameServerOwnerHandler : AuthorizationHandler<GameServerOwnerRequirement>
    {
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
                        if (httpContext.Request.Headers.TryGetValue("GameServerId", out var gameServerId))
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
