namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Whitelist Entry
    /// </summary>
    public class WhitelistEntryDto
    {
        /// <summary>
        /// Display Name
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Player Id
        /// </summary>
        public required string PlayerId { get; set; }
    }
}
