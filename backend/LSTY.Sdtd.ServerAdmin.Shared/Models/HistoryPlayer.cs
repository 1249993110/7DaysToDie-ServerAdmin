namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// History Player
    /// </summary>
    public class HistoryPlayer : PlayerBasicInfo
    {
        /// <summary>
        /// Permission Level
        /// </summary>
        public required int PermissionLevel { get; set; }

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
        public required IEnumerable<Position>? LandClaimBlocks { get; set; }

        /// <summary>
        /// Backpacks
        /// </summary>
        public required IEnumerable<Backpack>? Backpacks { get; set; }

        /// <summary>
        /// Bedroll
        /// </summary>
        public required Position? Bedroll { get; set; }

        /// <summary>
        /// QuestPositions
        /// </summary>
        public required IEnumerable<QuestpositionData>? QuestPositions { get; set; }

        /// <summary>
        /// Owned Vending Machine Positions
        /// </summary>
        public required IEnumerable<Position>? OwnedVendingMachinePositions { get; set; }
    }

    /// <summary>
    /// Backpack
    /// </summary>
    public class Backpack
    {
        /// <summary>
        /// Entity Id
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public required Position Position { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public required uint Timestamp { get; set; }
    }

    /// <summary>
    /// Questposition Data
    /// </summary>
    public class QuestpositionData
    {
        /// <summary>
        /// Quest Code
        /// </summary>
        public required int QuestCode { get; set; }

        /// <summary>
        /// Position Data Type
        /// </summary>
        public required string PositionDataType { get; set; }

        /// <summary>
        /// Block Position
        /// </summary>
        public required Position BlockPosition { get; set; }
    }
}
