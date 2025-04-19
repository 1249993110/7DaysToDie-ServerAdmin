using LSTY.Sdtd.ServerAdmin.Data.Notifications;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using MediatR;

namespace LSTY.Sdtd.ServerAdmin.Services.NotificationHandlers
{
    public class GameServerConnectedHander : INotificationHandler<GameServerConnected>
    {
        private readonly FunctionManager _functionManager;
        public GameServerConnectedHander(FunctionManager functionManager)
        {
            _functionManager = functionManager;
        }

        public async Task Handle(GameServerConnected notification, CancellationToken cancellationToken)
        {
            await _functionManager.RegisterFunctionsAsync(notification.GameServerId, notification.RpcProxies);
        }
    }
}
