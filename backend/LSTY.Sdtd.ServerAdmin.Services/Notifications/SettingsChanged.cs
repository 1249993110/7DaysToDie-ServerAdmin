using MediatR;

namespace LSTY.Sdtd.ServerAdmin.Services.Notifications
{
    public class SettingsChanged : INotification
    {
        public required string GameServerId { get; set; }
        public string? FunctionName { get; set; }
        public required IReadOnlyDictionary<string, object> Settings { get; set; }
    }
}
