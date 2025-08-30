namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Admin Entry
    /// </summary>
    public class AdminEntry
    {
        [Required]
        public required string PlayerId { get; set; }

        [DefaultValue(2000)]
        public required int PermissionLevel { get; set; } = 2000;

        public required string DisplayName { get; set; }
    }
}