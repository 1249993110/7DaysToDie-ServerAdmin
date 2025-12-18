namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerSpawningEventArgs : System.EventArgs
    {
        public required PlayerBasicInfoDto PlayerInfo { get; set; }

        public required int ChunkViewDim { get; set; }

        public required PlayerProfileDto PlayerProfile { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
