namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Player Details
    /// </summary>
    public class PlayerDetailsDto : PlayerBasicInfoDto
    {
        #region Online State
        /// <summary>
        /// Permission Level
        /// </summary>
        public required int PermissionLevel { get; set; }

        /// <summary>
        /// Is Admin
        /// </summary>
        public bool IsAdmin => PermissionLevel == 0;

        /// <summary>
        /// Is Twitch Enabled
        /// </summary>
        public required bool? IsTwitchEnabled { get; set; }

        /// <summary>
        /// Is Twitch Safe
        /// </summary>
        public required bool? IsTwitchSafe { get; set; }

        /// <summary>
        /// Zombie Kills
        /// </summary>
        public required int ZombieKills { get; set; }

        /// <summary>
        /// Player Kills
        /// </summary>
        public required int PlayerKills { get; set; }

        /// <summary>
        /// Deaths
        /// </summary>
        public required int Deaths { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        public required int Level { get; set; }

        /// <summary>
        /// Experience required to reach the next level.
        /// </summary>
        public required int ExpToNextLevel { get; set; }

        /// <summary>
        /// Skill Points
        /// </summary>
        public required int SkillPoints { get; set; }

        /// <summary>
        /// Game Stage
        /// </summary>
        public required int? GameStage { get; set; }
        #endregion

        #region History State
        /// <summary>
        /// Is Offline
        /// </summary>
        public bool IsOffline => EntityId == -1;

        /// <summary>
        /// Play Group
        /// </summary>
        public required string PlayGroup { get; set; }

        /// <summary>
        /// Last Login
        /// </summary>
        public required DateTime LastLogin { get; set; }

        /// <summary>
        /// ACL
        /// </summary>
        [JsonProperty("acl")]
        public required IEnumerable<string>? ACL { get; set; }

        /// <summary>
        /// Land Claim Blocks
        /// </summary>
        public required IEnumerable<PositionDto>? LandClaimBlocks { get; set; }

        /// <summary>
        /// Backpacks
        /// </summary>
        public required IEnumerable<Backpack>? Backpacks { get; set; }

        /// <summary>
        /// Bedroll
        /// </summary>
        public required PositionDto? Bedroll { get; set; }

        /// <summary>
        /// QuestPositions
        /// </summary>
        public required IEnumerable<QuestpositionData>? QuestPositions { get; set; }

        /// <summary>
        /// Owned Vending Machine Positions
        /// </summary>
        public required IEnumerable<PositionDto>? OwnedVendingMachinePositions { get; set; }
        #endregion

        #region Other State
        /// <summary>
        /// Last Spawn Position
        /// </summary>
        public required PositionDto LastSpawnPosition { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public required int Score { get; set; }

        /// <summary>
        /// Player Stats
        /// </summary>
        public required PlayerStatsDto? Stats { get; set; }

        /// <summary>
        /// Is Land Protection Active
        /// </summary>
        public required bool IsLandProtectionActive { get; set; }

        /// <summary>
        /// Distance Walked
        /// </summary>
        public required float DistanceWalked { get; set; }

        /// <summary>
        /// Total Items Crafted
        /// </summary>
        public required uint TotalItemsCrafted { get; set; }

        /// <summary>
        /// Longest Life
        /// </summary>
        public required float LongestLife { get; set; }

        /// <summary>
        /// Current Life
        /// </summary>
        public required float CurrentLife { get; set; }

        /// <summary>
        /// Total Time Played
        /// </summary>
        public required float TotalTimePlayed { get; set; }

        /// <summary>
        /// Rented VM Position
        /// </summary>
        public required PositionDto RentedVMPosition { get; set; }

        /// <summary>
        /// Rental End Time
        /// </summary>
        public required ulong RentalEndTime { get; set; }

        /// <summary>
        /// Rental End Day
        /// </summary>
        public required int RentalEndDay { get; set; }

        /// <summary>
        /// Spawn Points
        /// </summary>
        public required IEnumerable<PositionDto> SpawnPoints { get; set; }

        /// <summary>
        /// Already Crafted List
        /// </summary>
        public required IEnumerable<string> AlreadyCraftedList { get; set; }

        /// <summary>
        /// Unlocked Recipe List
        /// </summary>
        public required IEnumerable<string> UnlockedRecipeList { get; set; }

        /// <summary>
        /// Favorite Recipe List
        /// </summary>
        public required IEnumerable<string> FavoriteRecipeList { get; set; }

        /// <summary>
        /// Owned Entities
        /// </summary>
        public required IEnumerable<OwnedEntity> OwnedEntities { get; set; }

        /// <summary>
        /// Player Profile
        /// </summary>
        public required PlayerProfileDto? PlayerProfile { get; set; }
        #endregion
    }
}
