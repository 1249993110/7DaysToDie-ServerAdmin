namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class GameServerConfig
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [UniqueKey]
        public required Guid Id { get; set; }

        /// <summary>
        /// The date and time when the entity was created. If manually set, it should be in UTC format and must set ID of the entity.
        /// </summary>
        [DatabaseGenerated]
        public DateTime CreatedAt { get; set; }

        public required string UserId { get; set; }
        public required string Name { get; set; }
        public required string Ip { get; set; }
        public required int Port { get; set; }
        public required byte[] PfxFile { get; set; }
        public string? PfxPassword { get; set; }
        public required bool IsEnabled { get; set; }
        public string? Description { get; set; }
    }
}
