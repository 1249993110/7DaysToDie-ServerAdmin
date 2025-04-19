namespace LSTY.Sdtd.ServerAdmin.Overseer.Config
{
    internal class AppSettings
    {
        public int Port { get; set; }
        public required string CertPath { get; set; }
        public required string CertPassword { get; set; }
        public required string ServerSettingsFileName { get; set; }
    }
}
