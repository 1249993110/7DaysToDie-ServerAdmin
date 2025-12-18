using LSTY.Sdtd.ServerAdmin.Shared.Constants;

namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Entity Basic Info
    /// </summary>
    public class EntityBasicInfoDto
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
        public required PositionDto Position { get; set; }

        /// <summary>
        /// Entity Type
        /// </summary>
        public required EntityType EntityType { get; set; }

        /// <summary>
        /// Player Id
        /// </summary>
        public string? PlayerId { get; set; }
    }
}
