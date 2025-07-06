using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class ChatMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [UniqueKey, DatabaseGenerated]
        public int Id { get; set; }

        /// <summary>
        /// The date and time when the entity was created. If manually set, it should be in UTC format and must set ID of the entity.
        /// </summary>
        [IgnoreUpdate]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the game server.
        /// </summary>
        public required Guid GameServerId { get; set; }

        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        public string? PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the chat type.
        /// </summary>
        public required ChatType ChatType { get; set; }

        /// <summary>
        /// Gets or sets the sender's name.
        /// </summary>
        public required string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public required string Message { get; set; }
    }
}
