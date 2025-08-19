using LSTY.Sdtd.ServerAdmin.Config;
using LSTY.Sdtd.ServerAdmin.WebApi.DataProtection;
using LSTY.Sdtd.ServerAdmin.WebApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using System.Net;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Middlewares
{
    /// <summary>
    ///
    /// </summary>
    internal class SteamReturnMiddleware : OwinMiddleware
    {

        /// <summary>
        /// OAuth steam return path
        /// </summary>
        public const string OAuthSteamReturnPath = "/oauth/steam/return";

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        public SteamReturnMiddleware(OwinMiddleware next) : base(next)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path == new PathString(OAuthSteamReturnPath))
            {
                try
                {
                    // Handling Steam callback logic
                    var queryParams = context.Request.Query;

                    // 1. Get the parameters returned by Steam (such as `openid.claimed_id`, etc.)
                    string claimedId = queryParams["openid.claimed_id"];
                    string openidMode = queryParams["openid.mode"];
                    string redirect = queryParams["redirect"];

                    // 2. Verify that the parameters meet expectations (more validation logic can be added here)
                    if (string.IsNullOrEmpty(claimedId) || openidMode != "id_res")
                    {
                        string message = "Invalid Steam OAuth callback.";
                        string errorRedirectUrl = "/#/error?message=" + message;
                        context.Response.Redirect(errorRedirectUrl);
                        return;
                    }

                    // 3. Verify that Steam login is valid
                    bool isValid = VerifySteamLogin(context.Request.QueryString.ToString());
                    if (isValid == false)
                    {
                        string message = "Steam login verification failed.";
                        string errorRedirectUrl = "/#/error?message=" + message;
                        context.Response.Redirect(errorRedirectUrl);
                        return;
                    }

                    // 4. Extracting Steam ID
                    string playerName = string.Empty;
                    string steamId = ExtractSteamId(claimedId);
                    // EOS
                    var playerId = GetPlayerId(steamId);
                    if (playerId == null)
                    {
                        string message = "You are not a registered player.";
                        string errorRedirectUrl = "/#/403?message=" + message;
                        context.Response.Redirect(errorRedirectUrl);
                        return;
                    }

                    if (GameManager.Instance.persistentPlayers.Players.TryGetValue(playerId, out var playerData))
                    {
                        playerName = playerData.PlayerName.DisplayName;
                    }

                    bool isAdmin = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(playerId) == 0;
                    if (isAdmin == false)
                    {
                        string message = "You are not a level 0 administrator.";
                        string errorRedirectUrl = "/#/403?message=" + message;
                        context.Response.Redirect(errorRedirectUrl);
                        return;
                    }

                    // 5. Generate JWT Token or other necessary information
                    var ticket = CustomOAuthProvider.GenerateTicket(playerName);
                    string accessToken = new TicketDataFormat(CustomDataProtectionProvider.DataProtector).Protect(ticket);
                    string refreshToken = CustomRefreshTokenProvider.GenerateToken();
                    CustomRefreshTokenProvider.StorageToken(refreshToken, accessToken);

                    // 6. Redirect to the front-end page
                    string redirectUrl = $"/#/login/fromSteam?steamid={steamId}&playerName={playerName}&accessToken={accessToken}&expiresIn={AppConfig.Settings.AccessTokenExpireTime}&refreshToken={refreshToken}&redirect={redirect}";
                    context.Response.Redirect(redirectUrl);

                    return; // Make sure the next middleware is not called
                }
                catch (Exception ex)
                {
                    string message = $"Error: {ex.Message}";
                    string errorRedirectUrl = "/#/error?message=" + message;
                    context.Response.Redirect(errorRedirectUrl);
                    CustomLogger.Warn(ex.ToString());
                    return;
                }
            }

            // If the path is not /api/auth/steam/return, call the next middleware
            await Next.Invoke(context);
        }

        private static string ExtractSteamId(string claimedId)
        {
            if (string.IsNullOrEmpty(claimedId))
            {
                throw new ArgumentException("Claimed ID cannot be null or empty.");
            }

            // Checks if the URL starts with Steam's OpenID prefix
            const string steamOpenIdPrefix = "https://steamcommunity.com/openid/id/";
            if (claimedId.StartsWith(steamOpenIdPrefix, StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new ArgumentException("Invalid Steam Claimed ID format.");
            }

            return claimedId.Substring(steamOpenIdPrefix.Length);
        }

        private static PlatformUserIdentifierAbs? GetPlayerId(string steamId)
        {
            foreach (var item in GameManager.Instance.persistentPlayers.Players.Values)
            {
                if (item.NativeId.ReadablePlatformUserIdentifier == steamId)
                {
                    return item.PrimaryId;
                }
            }

            return null;
        }

        private static bool VerifySteamLogin(string queryString)
        {
            //queryString = Regex.Replace(queryString, "(?<=openid.mode=).+?(?=\\&)", "check_authentication", RegexOptions.IgnoreCase);

            int startIndex = queryString.IndexOf("openid.mode=", StringComparison.OrdinalIgnoreCase);
            if (startIndex != -1)
            {
                startIndex += "openid.mode=".Length;
                int endIndex = queryString.IndexOf("&", startIndex);
                if (endIndex == -1) endIndex = queryString.Length;

                // Replace openid.mode with "check_authentication"
                queryString = queryString.Substring(0, startIndex) + "check_authentication" + queryString.Substring(endIndex);
            }

            // Steam OpenID verification address
            string steamOpenIdUrl = "https://steamcommunity.com/openid/login" + queryString;

            using (WebClient webClient = new WebClient())
            {
                // webClient.Proxy = new WebProxy("http://127.0.0.1:10808");
                string str = webClient.DownloadString(steamOpenIdUrl);
                return str.Contains("is_valid:true");
            }
        }
    }
}