namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Represents the statistics of a player.
    /// </summary>
    public class PlayerStatsDto
    {
        /// <summary>
        /// Gets or sets the health of the player.
        /// </summary>
        public required float Health { get; set; }

        /// <summary>
        /// Gets or sets the stamina of the player.
        /// </summary>
        public required float Stamina { get; set; }

        /// <summary>
        /// Gets or sets the food level of the player.
        /// </summary>
        public required float Food { get; set; }

        /// <summary>
        /// Gets or sets the water level of the player.
        /// </summary>
        public required float Water { get; set; }
    }
}
