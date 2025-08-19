using LSTY.Sdtd.ServerAdmin.Config;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Providers
{
    internal class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.CompletedTask;
        }

        public static AuthenticationTicket GenerateTicket(string userName)
        {
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            DateTime utcNow = DateTime.UtcNow;
            var props = new AuthenticationProperties()
            {
                IssuedUtc = utcNow,
                ExpiresUtc = utcNow.AddSeconds(AppConfig.Settings.AccessTokenExpireTime),
            };

            return new AuthenticationTicket(identity, props);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (CheckCredential(context.UserName, context.Password) == false)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return Task.CompletedTask;
            }

            var ticket = GenerateTicket(context.UserName);
            context.Validated(ticket);

            return Task.CompletedTask;
        }

        private static bool CheckCredential(string userName, string password)
        {
            var appSettings = AppConfig.Settings;
            return userName == appSettings.UserName && password == appSettings.Password;
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var ticket = context.Ticket;

            // If the ticket's expiration time is earlier than the current time, the request is rejected.
            if (ticket.Properties.ExpiresUtc.HasValue && ticket.Properties.ExpiresUtc.Value < DateTime.UtcNow)
            {
                context.SetError("invalid_grant", "Refresh token has expired.");
                return Task.CompletedTask;
            }

            var newTicket = new AuthenticationTicket(ticket.Identity, ticket.Properties);
            context.Validated(newTicket);
            return Task.CompletedTask;
        }
    }
}