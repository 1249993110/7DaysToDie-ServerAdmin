﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
    {
        private string Challenge => $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            bool isWebSocketRequest = Context.WebSockets.IsWebSocketRequest;

            string authHeaderName = isWebSocketRequest ? HeaderNames.WebSocketSubProtocols : HeaderNames.Authorization;
            if (Request.Headers.ContainsKey(authHeaderName) == false)
            {
                Logger.LogDebug("No 'Authorization' header found in the request.");
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string? authHeaderValue = isWebSocketRequest ? Context.WebSockets.WebSocketRequestedProtocols.FirstOrDefault() : Request.Headers[authHeaderName].ToString();
            if (AuthenticationHeaderValue.TryParse(authHeaderValue, out var headerValue) == false)
            {
                Logger.LogDebug("No valid 'Authorization' header found in the request.");
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (headerValue.Scheme.Equals(AuthenticationSchemes.BasicAuthenticationScheme, StringComparison.OrdinalIgnoreCase) == false)
            {
                Logger.LogDebug($"'Authorization' header found but the scheme is not a '{AuthenticationSchemes.BasicAuthenticationScheme}' scheme.");
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            BasicCredentials credentials;
            try
            {
                credentials = DecodeBasicCredentials(headerValue.Parameter);
            }
            catch (Exception exception)
            {
                Logger.LogDebug(exception, "Error decoding credentials from header value.");
                return Task.FromResult(AuthenticateResult.Fail("Error decoding credentials from header value." + Environment.NewLine + exception.Message));
            }

            if (CheckPassword(credentials) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid userName or password."));
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, credentials.UserName),
                new Claim(ClaimTypes.Name, credentials.UserName),
            };
            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers[HeaderNames.WWWAuthenticate] = Challenge;
            await base.HandleChallengeAsync(properties);
        }

        private static BasicCredentials DecodeBasicCredentials(string? credentials)
        {
            if (credentials == null)
            {
                throw new Exception("Invalid Basic authentication header.");
            }

            string userName;
            string password;
            try
            {
                // Convert the base64 encoded 'userName:password' to normal string and parse userName and password from colon(:) separated string.
                var userNameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
                var userNameAndPasswordSplit = userNameAndPassword.Split(':');
                if (userNameAndPasswordSplit.Length != 2)
                {
                    throw new Exception("Invalid Basic authentication header.");
                }
                userName = userNameAndPasswordSplit[0];
                password = userNameAndPasswordSplit[1];
            }
            catch (Exception e)
            {
                throw new Exception($"Problem decoding '{AuthenticationSchemes.BasicAuthenticationScheme}' scheme credentials.", e);
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new Exception("UserName cannot be empty.");
            }

            if (password == null)
            {
                password = string.Empty;
            }

            return new BasicCredentials(userName, password);
        }

        private readonly struct BasicCredentials
        {
            public BasicCredentials(string userName, string password)
            {
                UserName = userName;
                Password = password;
            }

            public string UserName { get; }
            public string Password { get; }
        }

        private bool CheckPassword(BasicCredentials credentials)
        {
            return credentials.UserName.Equals(Options.UserName, StringComparison.InvariantCultureIgnoreCase) && credentials.Password == Options.Password;
        }
    }
}