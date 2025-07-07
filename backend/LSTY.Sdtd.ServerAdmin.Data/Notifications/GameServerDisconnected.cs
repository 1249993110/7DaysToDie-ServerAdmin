namespace LSTY.Sdtd.ServerAdmin.Data.Notifications
{
    public class GameServerDisconnected : INotification
    {
        public required Guid GameServerId { get; set; }
    }
}
