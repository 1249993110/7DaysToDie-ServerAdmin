namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Whitelist Entry
    /// </summary>
    public class WhitelistEntry
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
