using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authentication
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private string Challenge => $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";

        protected override Task<AuthenticationTicket?> AuthenticateCoreAsync()
        {
            try
            {
                var authHeader = Request.Headers.Get("Authorization");
                if (string.IsNullOrEmpty(authHeader) || authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase) == false)
                {
                    return Task.FromResult<AuthenticationTicket?>(null);
                }

                var credentials = DecodeBasicCredentials(authHeader.Substring("Basic ".Length).Trim());
                if (Options.Verifier.Invoke(credentials.UserName, credentials.Password) == false)
                {
                    return Task.FromResult<AuthenticationTicket?>(null);
                }

                var identity = new ClaimsIdentity(Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, credentials.UserName));
                identity.AddClaim(new Claim("role", "user"));

                return Task.FromResult<AuthenticationTicket?>(new AuthenticationTicket(identity, new AuthenticationProperties()));
            }
            catch
            {
                return Task.FromResult<AuthenticationTicket?>(null);
            }
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                var authenticationResponseChallenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
                if (authenticationResponseChallenge != null)
                {
                    Response.Headers.AppendValues("WWW-Authenticate", Challenge);
                }
            }
            return Task.CompletedTask;
        }


        private BasicCredentials DecodeBasicCredentials(string credentials)
        {
            if (string.IsNullOrEmpty(credentials))
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
                throw new Exception($"Problem decoding '{Options.AuthenticationType}' scheme credentials.", e);
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
    }
}
