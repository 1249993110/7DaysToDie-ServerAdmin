using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using MediatR;

namespace LSTY.Sdtd.ServerAdmin.Data.Notifications
{
    public class GameServerConnected : INotification
    {
        public required Guid GameServerId { get; set; }
        public required IReadOnlyDictionary<Type, IProxy> RpcProxies { get; set; }
    }
}
