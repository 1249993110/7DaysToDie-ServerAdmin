namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class PlayerDetails : PlayerInfo
    {
        /// <summary>
        /// Gets the player's admin status.
        /// </summary>
        public required bool IsAdmin { get; set; }

        /// <summary>
        /// Gets the last spawn position of the player.
        /// </summary>
        public required Position LastSpawnPosition { get; set; }

        /// <summary>
        /// Gets the last login time of the player.
        /// </summary>
        public required DateTime? LastLogin { get; set; }

        /// <summary>
        /// Gets the number of kills by the player.
        /// </summary>
        public required int PlayerKills { get; set; }

        /// <summary>
        /// Gets the number of zombie kills by the player.
        /// </summary>
        public required int ZombieKills { get; set; }

        /// <summary>
        /// Gets the number of deaths by the player.
        /// </summary>
        public required int Deaths { get; set; }

        /// <summary>
        /// Gets the score of the player.
        /// </summary>
        public required int Score { get; set; }

        /// <summary>
        /// Gets the player stats.
        /// </summary>
        public required PlayerStats? Stats { get; set; }

        /// <summary>
        /// Gets or sets the level of the player.
        /// </summary>
        public required int Level { get; set; }

        /// <summary>
        /// Gets or sets the experience required to reach the next level.
        /// </summary>
        public required int ExpToNextLevel { get; set; }

        /// <summary>
        /// Gets or sets the skill points available for the player.
        /// </summary>
        public required int SkillPoints { get; set; }

        /// <summary>
        /// Gets a value indicating whether land protection is active for the player.
        /// </summary>
        public required bool LandProtectionActive { get; set; }

        /// <summary>
        /// Gets the distance walked by the player.
        /// </summary>
        public required float DistanceWalked { get; set; }

        /// <summary>
        /// Gets the total number of items crafted by the player.
        /// </summary>
        public required uint TotalItemsCrafted { get; set; }

        /// <summary>
        /// Gets the longest life of the player.
        /// </summary>
        public required float LongestLife { get; set; }

        /// <summary>
        /// Gets the current life of the player.
        /// </summary>
        public required float CurrentLife { get; set; }

        /// <summary>
        /// Gets the total time played by the player in minutes.
        /// </summary>
        public required float TotalTimePlayed { get; set; }

        /// <summary>
        /// Gets the rented VM position of the player.
        /// </summary>
        public required Position RentedVMPosition { get; set; }

        /// <summary>
        /// Gets the rental end time of the player.
        /// </summary>
        public required ulong RentalEndTime { get; set; }

        /// <summary>
        /// Gets the rental end day of the player.
        /// </summary>
        public required int RentalEndDay { get; set; }

        /// <summary>
        /// Gets the spawn points of the player.
        /// </summary>
        public required IEnumerable<Position> SpawnPoints { get; set; }

        /// <summary>
        /// Gets the list of already crafted items by the player.
        /// </summary>
        public required IEnumerable<string> AlreadyCraftedList { get; set; }

        /// <summary>
        /// Gets the list of unlocked recipes by the player.
        /// </summary>
        public required IEnumerable<string> UnlockedRecipeList { get; set; }

        /// <summary>
        /// Gets the list of favorite recipes by the player.
        /// </summary>
        public required IEnumerable<string> FavoriteRecipeList { get; set; }

        /// <summary>
        /// Gets the list of owned entities by the player.
        /// </summary>
        public required IEnumerable<OwnedEntity> OwnedEntities { get; set; }
    }
}
