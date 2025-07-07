using LSTY.Sdtd.ServerAdmin.Data.Notifications;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using Microsoft.Extensions.Logging;

namespace LSTY.Sdtd.ServerAdmin.Services.NotificationHandlers
{
    public class GameServerConnectionStateChangeHander : INotificationHandler<GameServerConnected>, INotificationHandler<GameServerDisconnected>
    {
        private readonly ILogger<GameServerConnectionStateChangeHander> _logger;
        private readonly FunctionManager _functionManager;

        public GameServerConnectionStateChangeHander(FunctionManager functionManager, ILogger<GameServerConnectionStateChangeHander> logger)
        {
            _functionManager = functionManager;
            _logger = logger;
        }

        public async Task Handle(GameServerConnected notification, CancellationToken cancellationToken)
        {
            try
            {
                await _functionManager.RegisterFunctionsAsync(notification.GameServerId, notification.RpcProxies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register functions for game server {GameServerId}", notification.GameServerId);
            }
        }

        public async Task Handle(GameServerDisconnected notification, CancellationToken cancellationToken)
        {
            try
            {
                await _functionManager.UnregisterFunctionsAsync(notification.GameServerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unregister functions for game server {GameServerId}", notification.GameServerId);
            }
        }
    }
}
