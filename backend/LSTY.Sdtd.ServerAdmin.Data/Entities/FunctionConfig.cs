using LSTY.Sdtd.ServerAdmin.Data.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class FunctionConfig
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [UniqueKey, DatabaseGenerated]
        public int Id { get; set; }

        /// <summary>
        /// The date and time when the entity was created. If manually set, it should be in UTC format and must set ID of the entity.
        /// </summary>
        [DatabaseGenerated]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the game server.
        /// </summary>
        public required Guid GameServerId { get; set; }

        /// <summary>
        /// Function name.
        /// </summary>
        /// <remarks>
        /// Use `CommonSettings` for common settings.
        /// </remarks>
        public required string FunctionName { get; set; }

        /// <summary>
        /// Serialized settings.
        /// </summary>
        public required string Settings { get; set; }
    }
}
