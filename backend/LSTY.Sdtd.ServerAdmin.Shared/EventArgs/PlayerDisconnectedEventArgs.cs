using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerDisconnectedEventArgs : System.EventArgs
    {
        public required PlayerInfo PlayerInfo { get; set; }

        public required bool Shutdown { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
