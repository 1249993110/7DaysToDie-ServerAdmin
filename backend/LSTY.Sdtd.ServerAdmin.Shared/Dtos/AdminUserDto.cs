using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Admin User
    /// </summary>
    public class AdminUserDto
    {
        [Required]
        public required string PlayerId { get; set; }

        [DefaultValue(2000)]
        public required int PermissionLevel { get; set; } = 2000;

        public required string DisplayName { get; set; }
    }
}