using LSTY.Sdtd.ServerAdmin.Data.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class GameServerConfig : FileEntityBase
    {
        /// <summary>
        /// The default file name for the PFX certificate.
        /// </summary>
        public const string PfxFileName = "cert.pfx";

        public required string Name { get; set; }
        public required string Ip { get; set; }
        public required int Port { get; set; }
        public required string? PfxPassword { get; set; }
        public required bool IsEnabled { get; set; }

        public required string UserId { get; set; }

        static GameServerConfig()
        {
            DB.Index<GameServerConfig>()
                .Key(e => e.IsEnabled, KeyType.Ascending)
                .CreateAsync();
        }
    }
}
