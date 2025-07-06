using LSTY.Sdtd.ServerAdmin.Data.Enums;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class LogEntry
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

        public required Guid GameServerId { get; set; }

        public required ServiceModule ServiceModule { get; set; }

        public required LogLevel LogLevel { get; set; }

        public required string Message { get; set; }

        public string? Exception { get; set; }

        public required Guid CorrelationId { get; set; }
    }
}
