using Microsoft.AspNetCore.Authentication;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Authentication
{
    /// <summary>
    ///
    /// </summary>
    public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Realm { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Password { get; set; } = null!;
    }
}