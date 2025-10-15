namespace LSTY.Sdtd.ServerAdmin.Config
{
    public class AppSettings
    {
        public required string WebUrl { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required int AccessTokenExpireTime { get; set; }
        public required int RefreshTokenExpireTime { get; set; }
        public required string ServerConfigFile { get; set; }
    }
}
