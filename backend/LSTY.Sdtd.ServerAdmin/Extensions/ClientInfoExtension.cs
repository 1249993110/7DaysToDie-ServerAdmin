using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class ClientInfoExtension
    {
        public static PlayerBasicInfo ToPlayerBasicInfo(this ClientInfo clientInfo, Position? position = null)
        {
            return new PlayerBasicInfo()
            {
                EntityId = clientInfo.entityId,
                PlayerId = clientInfo.CrossplatformId.CombinedString,
                PlatformId = clientInfo.PlatformId.CombinedString,
                PlayerName = clientInfo.playerName,
                Position = position ?? default,
                Ip = clientInfo.ip,
                Ping = clientInfo.ping,
            };
        }
    }
}