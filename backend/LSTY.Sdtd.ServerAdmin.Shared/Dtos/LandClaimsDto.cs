namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Land Claims
    /// </summary>
    public class LandClaimsDto
    {
        /// <summary>
        /// Claim Owners
        /// </summary>
        public required IEnumerable<ClaimOwnerDto> ClaimOwners { get; set; }

        /// <summary>
        /// Claim Size
        /// </summary>
        public required int ClaimSize { get; set; }
    }
}
