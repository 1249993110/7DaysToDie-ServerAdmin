namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Entity Basic Info
    /// </summary>
    public class EntityBasicInfo
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Entity Name
        /// </summary>
        public required string EntityName { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public required Position Position { get; set; }

        /// <summary>
        /// Entity Type
        /// </summary>
        public required EntityType EntityType { get; set; }
    }
}
