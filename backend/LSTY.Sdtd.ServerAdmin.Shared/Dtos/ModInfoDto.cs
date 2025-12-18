namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Mod Info
    /// </summary>
    public class ModInfoDto
    {
        public required string Name { get; set; }
        public required string DisplayName { get; set; }
        public required string Description { get; set; }
        public required string Author { get; set; }
        public required string Website { get; set; }
        public required string Version { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsUninstalled { get; set; }
        public required string FolderName { get; set; }

    }
}
