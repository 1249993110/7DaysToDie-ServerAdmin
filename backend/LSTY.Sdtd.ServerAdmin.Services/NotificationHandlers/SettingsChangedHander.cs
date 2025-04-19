using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.Services.Notifications;
using MediatR;

namespace LSTY.Sdtd.ServerAdmin.Services.NotificationHandlers
{
    public class SettingsChangedHander : INotificationHandler<SettingsChanged>
    {
        private readonly FunctionManager _functionManager;
        public SettingsChangedHander(FunctionManager functionManager)
        {
            _functionManager = functionManager;
        }

        public Task Handle(SettingsChanged notification, CancellationToken cancellationToken)
        {
            _functionManager.UpdateFunctionSettings(notification.GameServerId, notification.FunctionName, notification.Settings);
            return Task.CompletedTask;
        }        
    }
}
