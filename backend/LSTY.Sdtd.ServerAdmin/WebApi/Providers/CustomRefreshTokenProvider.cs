using LSTY.Sdtd.ServerAdmin.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Owin.Security.Infrastructure;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Providers
{
    internal class CustomRefreshTokenProvider : AuthenticationTokenProvider
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        internal static string StorageToken(string refreshToken, string accessToken)
        {
            _cache.Set(refreshToken, accessToken, TimeSpan.FromSeconds(AppConfig.Settings.RefreshTokenExpireTime));

            return refreshToken;
        }

        public static string GenerateToken()
        {
            return Guid.NewGuid().ToString("n");
        }

        public override void Create(AuthenticationTokenCreateContext context)
        {
            var refreshToken = GenerateToken();

            DateTime utcNow = DateTime.UtcNow;
            context.Ticket.Properties.IssuedUtc = utcNow;
            context.Ticket.Properties.ExpiresUtc = utcNow.AddSeconds(AppConfig.Settings.RefreshTokenExpireTime);
            context.SetToken(refreshToken);

            StorageToken(refreshToken, context.SerializeTicket());
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // Verify the logic of Refresh Token (retrieve the token from the database or cache)
            var refreshToken = context.Token;

            // If the verification is successful, the Access Token is regenerated
            if (string.IsNullOrEmpty(refreshToken) == false && _cache.TryGetValue(refreshToken, out string? accessToken))
            {
                context.DeserializeTicket(accessToken);
                _cache.Remove(refreshToken);
            }
        }
    }

}
