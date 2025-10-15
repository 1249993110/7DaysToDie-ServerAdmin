namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Command Permission Create
    /// </summary>
    public class CommandPermissionCreate
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
    public class CommandPermission : CommandPermissionCreate
    {
        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }
    }
}
