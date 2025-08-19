using LSTY.Sdtd.ServerAdmin.WebApi.Authentication;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Middlewares
{
    internal class BasicAuthenticationMiddleware : AuthenticationMiddleware<BasicAuthenticationOptions>
    {
        public BasicAuthenticationMiddleware(OwinMiddleware next, BasicAuthenticationOptions options)
            : base(next, options)
        {
        }

        protected override AuthenticationHandler<BasicAuthenticationOptions> CreateHandler()
        {
            return new BasicAuthenticationHandler();
        }
    }
}
