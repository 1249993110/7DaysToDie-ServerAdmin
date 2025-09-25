namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Land Claims
    /// </summary>
    public class LandClaims
    {
        /// <summary>
        /// Claim Owners
        /// </summary>
        public required IEnumerable<ClaimOwner> ClaimOwners { get; set; }

        /// <summary>
        /// Claim Size
        /// </summary>
        public required int ClaimSize { get; set; }
    }
}
