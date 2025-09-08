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
                Position = position ?? clientInfo.latestPlayerData.ecd.pos.ToPosition(),
                Ip = clientInfo.ip,
                Ping = clientInfo.ping,
            };
        }

        //public static Shared.Models.PlayerDetails ToPlayerDetails(this ClientInfo clientInfo)
        //{
        //    var playerDataFile = clientInfo.latestPlayerData;
        //    GameManager.Instance.persistentPlayers.Players.TryGetValue(clientInfo.CrossplatformId, out var persistentPlayerData);
        //    GameManager.Instance.World.Players.dict.TryGetValue(clientInfo.entityId, out var entityPlayer);

        //    var stats = playerDataFile.ecd.stats;

        //    int level = 0;
        //    int expToNextLevel = 0;
        //    int skillPoints = 0;
        //    if (entityPlayer != null)
        //    {
        //        level = entityPlayer.Progression.Level;
        //        expToNextLevel = entityPlayer.Progression.ExpToNextLevel;
        //        skillPoints = entityPlayer.Progression.SkillPoints;
        //    }
        //    else
        //    {
        //        var stream = playerDataFile.progressionData;
        //        if (stream.Length > 0L)
        //        {
        //            using var binaryReader = MemoryPools.poolBinaryReader.AllocSync(false);
        //            stream.Position = 0L;
        //            binaryReader.SetBaseStream(stream);

        //            byte b = binaryReader.ReadByte();
        //            level = binaryReader.ReadUInt16();
        //            skillPoints = binaryReader.ReadInt32();
        //            skillPoints = binaryReader.ReadUInt16();
        //        }
        //    }

        //    return new Shared.Models.PlayerDetails()
        //    {
        //        EntityId = clientInfo.entityId,
        //        EntityName = clientInfo.playerName,
        //        EntityType = Shared.Models.EntityType.OnlinePlayer,
        //        PlatformId = clientInfo.PlatformId.CombinedString,
        //        PlayerId = clientInfo.CrossplatformId.CombinedString,
        //        IsAdmin = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(clientInfo.CrossplatformId) == 0,
        //        Ip = clientInfo.ip,
        //        Ping = clientInfo.ping,
        //        GameStage = entityPlayer?.gameStage,
        //        Position = entityPlayer?.position.ToPosition() ?? playerDataFile.ecd.pos.ToPosition(),
        //        LastSpawnPosition = playerDataFile.lastSpawnPosition.position.ToPosition(),
        //        LastLogin = persistentPlayerData?.LastLogin,
        //        PlayerKills = playerDataFile.playerKills,
        //        ZombieKills = playerDataFile.zombieKills,
        //        Deaths = playerDataFile.deaths,
        //        Score = playerDataFile.score,
        //        Stats = stats == null ? null : new PlayerStats()
        //        {
        //            Health = stats.Health.Value,
        //            Stamina = stats.Stamina.Value,
        //            Water = stats.Water.Value,
        //            Food = stats.Food.Value,
        //        },
        //        Level = level,
        //        ExpToNextLevel = expToNextLevel,
        //        SkillPoints = skillPoints,
        //        LandProtectionActive = persistentPlayerData != null && GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerData),
        //        DistanceWalked = playerDataFile.distanceWalked,
        //        TotalItemsCrafted = playerDataFile.totalItemsCrafted,
        //        LongestLife = playerDataFile.longestLife,
        //        CurrentLife = playerDataFile.currentLife,
        //        TotalTimePlayed = playerDataFile.totalTimePlayed,
        //        RentedVMPosition = playerDataFile.rentedVMPosition.ToPosition(),
        //        RentalEndTime = playerDataFile.rentalEndTime,
        //        RentalEndDay = playerDataFile.rentalEndDay,
        //        SpawnPoints = playerDataFile.spawnPoints.ToPositions(),
        //        AlreadyCraftedList = playerDataFile.alreadyCraftedList,
        //        UnlockedRecipeList = playerDataFile.unlockedRecipeList,
        //        FavoriteRecipeList = playerDataFile.favoriteRecipeList,
        //        OwnedEntities = playerDataFile.ownedEntities.ToModels(),
        //    };
        //}
    }
}