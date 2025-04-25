using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class ChatMessage : EntityBase
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        public string? PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the sender's name.
        /// </summary>
        public required string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the chat type.
        /// </summary>
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required ChatType ChatType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public required string Message { get; set; }

        public required string GameServerId { get; set; }

        static ChatMessage()
        {
            DB.Index<ChatMessage>()
                .Key(e => e.GameServerId, KeyType.Ascending)
                .Key(e => e.CreatedOn, KeyType.Descending)
                .Key(e => e.EntityId, KeyType.Ascending)
                .CreateAsync();

            DB.Index<ChatMessage>()
                .Key(e => e.GameServerId, KeyType.Ascending)
                .Key(e => e.CreatedOn, KeyType.Descending)
                .Key(e => e.PlayerId, KeyType.Ascending)
                .CreateAsync();

            DB.Index<ChatMessage>()
                .Key(e => e.GameServerId, KeyType.Ascending)
                .Key(e => e.CreatedOn, KeyType.Descending)
                .Key(e => e.ChatType, KeyType.Ascending)
                .CreateAsync();

            DB.Index<ChatMessage>()
                .Key(e => e.GameServerId, KeyType.Ascending)
                .Key(e => e.CreatedOn, KeyType.Descending)
                .Key(e => e.SenderName, KeyType.Ascending)
                .CreateAsync();
        }
    }
}
