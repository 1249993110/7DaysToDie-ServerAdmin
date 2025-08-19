namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Admin Entry
    /// </summary>
    public class AdminEntry
    {
        public required string PlayerId { get; set; }

        public required int PermissionLevel { get; set; }

        public required string DisplayName { get; set; }
    }
}