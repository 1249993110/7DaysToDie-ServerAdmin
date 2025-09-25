namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Represents the basic information of a player.
    /// </summary>
    public class PlayerBasicInfo
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Player Id
        /// </summary>
        public required string PlayerId { get; set; }

        /// <summary>
        /// Platform Id
        /// </summary>
        public required string PlatformId { get; set; }

        /// <summary>
        /// Player Name
        /// </summary>
        public required string PlayerName { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public required Position Position { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Ping
        /// </summary>
        public int? Ping { get; set; }
    }
}
