namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Online Player
    /// </summary>
    public class OnlinePlayerDto : PlayerBasicInfoDto
    {
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
        public required bool IsTwitchEnabled { get; set; }

        /// <summary>
        /// Is Twitch Safe
        /// </summary>
        public required bool IsTwitchSafe { get; set; }

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
        public required int GameStage { get; set; }
    }
}
