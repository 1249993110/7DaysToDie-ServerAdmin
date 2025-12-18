namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Claim Owner
    /// </summary>
    public class ClaimOwnerDto : PlayerBasicInfoDto
    {
        /// <summary>
        /// Claim Active
        /// </summary>
        public required bool ClaimActive { get; set; }

        /// <summary>
        /// Last Login
        /// </summary>
        public required DateTime LastLogin { get; set; }

        /// <summary>
        /// Claim Positions
        /// </summary>
        public required IEnumerable<PositionDto> ClaimPositions { get; set; }
    }
}
