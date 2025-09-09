using LSTY.Sdtd.ServerAdmin.Shared.Models;
using static PersistentPlayerData;

namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class PersistentPlayerDataExtension
    {
        public static HistoryPlayer ToHistoryPlayer(this PersistentPlayerData persistentPlayerData, ClientInfo? clientInfo)
        {
            return new HistoryPlayer()
            {
                EntityId = persistentPlayerData.EntityId,
                PlayerId = persistentPlayerData.PrimaryId.CombinedString,
                PlatformId = persistentPlayerData.NativeId.CombinedString,
                PlayerName = persistentPlayerData.PlayerName.DisplayName,
                Position = persistentPlayerData.Position.ToPosition(),
                Ip = clientInfo?.ip,
                Ping = clientInfo?.ping,
                PermissionLevel = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(persistentPlayerData.PrimaryId),
                PlayGroup = persistentPlayerData.PlayGroup.ToString(),
                LastLogin = persistentPlayerData.LastLogin,
                ACL = persistentPlayerData.ACL?.Select(i => i.CombinedString),
                LandClaimBlocks = persistentPlayerData.LPBlocks?.Select(i => i.ToPosition()),
                Backpacks = persistentPlayerData.backpacksByID?.Select(i => i.ToBackpack()),
                Bedroll = persistentPlayerData.BedrollPos.y == int.MaxValue ? null : persistentPlayerData.BedrollPos.ToPosition(),
                QuestPositions = persistentPlayerData.QuestPositions?.Select(i => i.ToQuestpositionData()),
                OwnedVendingMachinePositions = persistentPlayerData.OwnedVendingMachinePositions?.Select(i => i.ToPosition()),
            };
        }

        public static Shared.Models.PlayerDetails ToPlayerDetails(this PersistentPlayerData persistentPlayerData, ClientInfo? clientInfo, EntityPlayer? entityPlayer)
        {
            PlayerDataFile playerDataFile;
            if (clientInfo != null)
            {
                playerDataFile = clientInfo.latestPlayerData;
            }
            else
            {
                playerDataFile = new PlayerDataFile();
                playerDataFile.Load(GameIO.GetPlayerDataDir(), persistentPlayerData.PrimaryId.CombinedString);
            }

            var result = new Shared.Models.PlayerDetails()
            {
                EntityId = persistentPlayerData.EntityId,
                PlayerId = persistentPlayerData.PrimaryId.CombinedString,
                PlatformId = persistentPlayerData.NativeId.CombinedString,
                PlayerName = persistentPlayerData.PlayerName.DisplayName,
                Position = persistentPlayerData.Position.ToPosition(),
                Ip = clientInfo?.ip,
                Ping = clientInfo?.ping,
                PermissionLevel = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(persistentPlayerData.PrimaryId),
                PlayGroup = persistentPlayerData.PlayGroup.ToString(),
                LastLogin = persistentPlayerData.LastLogin,
                ACL = persistentPlayerData.ACL?.Select(i => i.CombinedString),
                LandClaimBlocks = persistentPlayerData.LPBlocks?.Select(i => i.ToPosition()),
                Backpacks = persistentPlayerData.backpacksByID?.Select(i => i.ToBackpack()),
                Bedroll = persistentPlayerData.BedrollPos.y == int.MaxValue ? null : persistentPlayerData.BedrollPos.ToPosition(),
                QuestPositions = persistentPlayerData.QuestPositions?.Select(i => i.ToQuestpositionData()),
                OwnedVendingMachinePositions = persistentPlayerData.OwnedVendingMachinePositions?.Select(i => i.ToPosition()),

                IsTwitchEnabled = null,
                IsTwitchSafe = null,
                ZombieKills = playerDataFile.zombieKills,
                PlayerKills = playerDataFile.playerKills,
                Deaths = playerDataFile.deaths,
                Level = 0,
                ExpToNextLevel = 0,
                SkillPoints = 0,
                GameStage = null,

                LastSpawnPosition = playerDataFile.lastSpawnPosition.position.ToPosition(),
                Score = playerDataFile.score,
                Stats = null,
                IsLandProtectionActive = GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerData),
                DistanceWalked = playerDataFile.distanceWalked,
                TotalItemsCrafted = playerDataFile.totalItemsCrafted,
                LongestLife = playerDataFile.longestLife,
                CurrentLife = playerDataFile.currentLife,
                TotalTimePlayed = playerDataFile.totalTimePlayed,
                RentedVMPosition = playerDataFile.rentedVMPosition.ToPosition(),
                RentalEndTime = playerDataFile.rentalEndTime,
                RentalEndDay = playerDataFile.rentalEndDay,
                SpawnPoints = playerDataFile.spawnPoints.ToPositions(),
                AlreadyCraftedList = playerDataFile.alreadyCraftedList,
                UnlockedRecipeList = playerDataFile.unlockedRecipeList,
                FavoriteRecipeList = playerDataFile.favoriteRecipeList,
                OwnedEntities = playerDataFile.ownedEntities.ToModels(),
                PlayerProfile = null,
            };

            if (entityPlayer == null)
            {
                var stream = playerDataFile.progressionData;
                if (stream.Length > 0L)
                {
                    using var binaryReader = MemoryPools.poolBinaryReader.AllocSync(false);
                    stream.Position = 0L;
                    binaryReader.SetBaseStream(stream);

                    byte b = binaryReader.ReadByte();
                    result.Level = binaryReader.ReadUInt16();
                    result.ExpToNextLevel = binaryReader.ReadInt32();
                    result.SkillPoints = binaryReader.ReadUInt16();
                }
            }
            else
            {
                var progression = entityPlayer.Progression;
                result.Level = progression == null ? 0 : progression.Level;
                result.ExpToNextLevel = progression == null ? 0 : progression.ExpToNextLevel;
                result.SkillPoints = progression == null ? 0 : progression.SkillPoints;

                result.IsTwitchEnabled = entityPlayer.TwitchEnabled && entityPlayer.twitchActionsEnabled == EntityPlayer.TwitchActionsStates.Enabled;
                result.IsTwitchSafe = entityPlayer.twitchSafe;
                result.GameStage = entityPlayer.gameStage;
                result.PlayerProfile = entityPlayer.playerProfile.ToModel();
            }

            var stats = playerDataFile.ecd.stats;
            if (stats != null)
            {
                result.Stats = new PlayerStats()
                {
                    Health = stats.Health.Value,
                    Stamina = stats.Stamina.Value,
                    // CoreTemp = stats.CoreTemp.Value,
                    Food = stats.Food.Value,
                    Water = stats.Water.Value
                };
            }

            return result;
        }

        private static Backpack ToBackpack(this KeyValuePair<int, ProtectedBackpack> kv)
        {
            return new Backpack()
            {
                EntityId = kv.Key,
                Position = kv.Value.Position.ToPosition(),
                Timestamp = kv.Value.Timestamp,
            };
        }

        private static QuestpositionData ToQuestpositionData(this global::QuestPositionData questposition)
        {
            return new QuestpositionData()
            {
                QuestCode = questposition.questCode,
                BlockPosition = questposition.blockPosition.ToPosition(),
                PositionDataType = questposition.positionDataType.ToString(),
            };
        }
    }
}
