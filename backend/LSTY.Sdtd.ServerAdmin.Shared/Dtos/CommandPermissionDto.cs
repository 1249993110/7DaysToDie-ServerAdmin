namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Command Permission Create
    /// </summary>
    public class CommandPermissionCreateDto
    {
        /// <summary>
        /// Command
        /// </summary>
        public required string Command { get; set; }

        /// <summary>
        /// Permission Level
        /// </summary>
        public int PermissionLevel { get; set; }
    }

    /// <summary>
    /// Command Permission
    /// </summary>
    public class CommandPermissionDto : CommandPermissionCreateDto
    {
        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }
    }
}
