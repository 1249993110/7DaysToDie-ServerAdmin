using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerLoginEventArgs : System.EventArgs
    {
        public required PlayerInfo PlayerInfo { get; set; }

        public required string CompatibilityVersion { get; set; }

        public required string CustomMessage { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
