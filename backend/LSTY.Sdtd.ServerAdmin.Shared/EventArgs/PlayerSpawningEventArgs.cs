using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerSpawningEventArgs : System.EventArgs
    {
        public required PlayerBasicInfo PlayerInfo { get; set; }

        public required int ChunkViewDim { get; set; }

        public required PlayerProfile PlayerProfile { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
