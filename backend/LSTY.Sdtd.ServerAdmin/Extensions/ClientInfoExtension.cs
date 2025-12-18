

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class ClientInfoExtension
    {
        public static PlayerBasicInfoDto ToPlayerBasicInfo(this ClientInfo clientInfo, PositionDto? position = null)
        {
            return new PlayerBasicInfoDto()
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