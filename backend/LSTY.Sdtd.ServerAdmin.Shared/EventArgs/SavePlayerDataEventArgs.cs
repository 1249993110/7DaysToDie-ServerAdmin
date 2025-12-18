namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class SavePlayerDataEventArgs : System.EventArgs
    {
        public required PlayerDetailsDto PlayerDetails { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
