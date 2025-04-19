using LSTY.Sdtd.ServerAdmin.Data.Notifications;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using MediatR;

namespace LSTY.Sdtd.ServerAdmin.Services.NotificationHandlers
{
    public class GameServerDisconnectedHander : INotificationHandler<GameServerDisconnected>
    {
        private readonly FunctionManager _functionManager;
        public GameServerDisconnectedHander(FunctionManager functionManager)
        {
            _functionManager = functionManager;
        }

        public Task Handle(GameServerDisconnected notification, CancellationToken cancellationToken)
        {
            _functionManager.UnregisterFunctions(notification.GameServerId);
            return Task.CompletedTask;
        }
    }
}
