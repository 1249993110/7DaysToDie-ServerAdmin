namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Claim Owner
    /// </summary>
    public class ClaimOwner : PlayerBasicInfo
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
        public required IEnumerable<Position> ClaimPositions { get; set; }
    }
}
