using Microsoft.Owin.Security;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authentication
{
    internal class BasicAuthenticationOptions : AuthenticationOptions
    {
        public required string Realm { get; set; }

        public required Func<string, string, bool> Verifier { get; set; }

        public BasicAuthenticationOptions() : base(AuthenticationSchemes.Basic)
        {
        }
    }
}