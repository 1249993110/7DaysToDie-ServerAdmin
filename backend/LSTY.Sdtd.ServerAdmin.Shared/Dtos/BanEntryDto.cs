namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Ban Entry
    /// </summary>
    public class BanEntryDto
    {
        /// <summary>
        /// Unban date
        /// </summary>
        public DateTime BannedUntil { get; set; }

        /// <summary>
        /// Display name, defaults to the player's nickname
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Player ID (platform ID or cross-platform ID, format: platform type + ID, such as EOS_XXXX or Steam_XXXX)
        /// </summary>
        public required string PlayerId { get; set; }

        /// <summary>
        /// Ban reason
        /// </summary>
        public string? Reason { get; set; }
    }
}
