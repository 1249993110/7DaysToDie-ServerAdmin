

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class EntityPlayerExtension
    {
        public static OnlinePlayerDto ToOnlinePlayer(this EntityPlayer entityPlayer, ClientInfo clientInfo)
        {
            var progression = entityPlayer.Progression;
            return new OnlinePlayerDto()
            {
                EntityId = clientInfo.entityId,
                PlayerId = clientInfo.CrossplatformId.CombinedString,
                PlatformId = clientInfo.PlatformId.CombinedString,
                PlayerName = clientInfo.playerName,
                Position = entityPlayer.position.ToPosition(),
                Ip = clientInfo.ip,
                Ping = clientInfo.ping,
                PermissionLevel = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(clientInfo),
                IsTwitchEnabled = entityPlayer.TwitchEnabled && entityPlayer.twitchActionsEnabled == EntityPlayer.TwitchActionsStates.Enabled,
                IsTwitchSafe = entityPlayer.TwitchSafe,
                ZombieKills = entityPlayer.killedZombies,
                PlayerKills = entityPlayer.killedPlayers,
                Deaths = entityPlayer.died,
                Level = progression == null ? 0 : progression.Level,
                ExpToNextLevel = progression == null ? 0 : progression.ExpToNextLevel,
                SkillPoints = progression == null ? 0 : progression.SkillPoints,
                GameStage = entityPlayer.gameStage,
            };
        }
    }
}
