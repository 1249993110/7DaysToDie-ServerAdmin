namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Represents the base information of a player.
    /// </summary>
    public class PlayerInfo : EntityInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public required string PlayerId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required string PlatformId { get; set; }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Gets the ping value.
        /// </summary>
        [DefaultValue(-1)]
        public int Ping { get; set; } = -1;

        /// <summary>
        /// Gets the game stage of the player.
        /// </summary>
        [DefaultValue(-1)]
        public int GameStage { get; set; } = -1;

        /// <summary>
        /// Gets a value indicating whether the player is offline.
        /// </summary>
        public bool IsOffline => EntityType == EntityType.OfflinePlayer;
    }
}
