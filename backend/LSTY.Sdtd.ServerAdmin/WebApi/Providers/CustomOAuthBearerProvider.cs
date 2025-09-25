using Microsoft.Owin.Security.OAuth;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Providers
{
    internal class CustomOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                return Task.CompletedTask;
            }

            string token = context.Request.Query.Get("access_token");
            if (string.IsNullOrEmpty(token) == false)
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    }
}
