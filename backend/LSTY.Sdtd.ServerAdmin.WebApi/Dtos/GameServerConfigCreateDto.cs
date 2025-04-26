namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// Game Server Config Create DTO.
    /// </summary>
    public class GameServerConfigCreateDto
    {
        /// <summary>
        /// The name of the game server.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The IP address of the game server.
        /// </summary>
        public required string Ip { get; set; }

        /// <summary>
        /// The port number used by the game server.
        /// </summary>
        public required int Port { get; set; }

        /// <summary>
        /// The password for the PFX certificate, if applicable.
        /// </summary>
        public string? PfxPassword { get; set; }

        /// <summary>
        /// A value indicating whether the game server is enabled.
        /// </summary>
        public required bool IsEnabled { get; set; }

        /// <summary>
        /// The optional description of the game server.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The PFX file used for the game server.
        /// </summary>
        public required IFormFile PfxFile { get; set; }
    }
}
