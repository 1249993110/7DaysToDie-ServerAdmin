using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerSpawnedInWorldEventArgs : System.EventArgs
    {
        public required PlayerInfo PlayerInfo { get; set; }

        public RespawnType RespawnType { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
