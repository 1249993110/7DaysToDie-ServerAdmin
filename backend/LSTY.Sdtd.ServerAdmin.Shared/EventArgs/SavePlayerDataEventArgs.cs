using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class SavePlayerDataEventArgs : System.EventArgs
    {
        public required PlayerDetails PlayerDetails { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
