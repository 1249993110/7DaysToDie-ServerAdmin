using System.Security.Claims;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Extensions
{
    internal static class ClaimsPrincipalExtension
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new InvalidOperationException("User ID claim not found.");
            return claim.Value;
        }
    }
}
