namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class GameServerConfig : FileEntity, ICreatedOn, IModifiedOn
    {
        /// <summary>
        /// The default file name for the PFX certificate.
        /// </summary>
        public const string PfxFileName = "cert.pfx";

        public required DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public required string Name { get; set; }
        public required string Ip { get; set; }
        public int Port { get; set; }
        public string? PfxPassword { get; set; }
        public bool IsEnabled { get; set; }
    }
}
