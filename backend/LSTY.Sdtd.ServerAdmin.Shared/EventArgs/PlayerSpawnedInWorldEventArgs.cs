using LSTY.Sdtd.ServerAdmin.Shared.Constants;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerSpawnedInWorldEventArgs : System.EventArgs
    {
        public required PlayerBasicInfoDto PlayerInfo { get; set; }

        public RespawnType RespawnType { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
