namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class CommandSender
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        public required string PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the sender's name.
        /// </summary>
        public required string PlayerName { get; set; }
    }
}
