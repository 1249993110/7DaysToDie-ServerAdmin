namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerDisconnectedEventArgs : System.EventArgs
    {
        public required PlayerBasicInfoDto PlayerInfo { get; set; }

        public required bool GameShuttingDown { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
