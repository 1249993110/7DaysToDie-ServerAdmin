namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Admin User
    /// </summary>
    public class AdminUser
    {
        [Required]
        public required string PlayerId { get; set; }

        [DefaultValue(2000)]
        public required int PermissionLevel { get; set; } = 2000;

        public required string DisplayName { get; set; }
    }
}