using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class ChatMessageEventArgs : System.EventArgs
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
        public required ChatType ChatType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int>? RecipientEntityIds { get; set; }
    }
}
