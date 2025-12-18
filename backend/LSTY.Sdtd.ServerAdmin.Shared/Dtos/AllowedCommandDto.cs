namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// The game server allowed command
    /// </summary>
    public class AllowedCommandDto
    {
        /// <summary>
        /// Commands
        /// </summary>
        public required IEnumerable<string> Commands { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Help
        /// </summary>
        public required string Help { get; set; }

        /// <summary>
        /// Permission level
        /// </summary>
        public int PermissionLevel { get; set; }
    }
}
