namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Inventory Item
    /// </summary>
    public class InvItemDto
    {
        /// <summary>
        /// Item Name
        /// </summary>
        public required string ItemName { get; set; }

        /// <summary>
        /// Icon Name
        /// </summary>
        public required string IconName { get; set; }

        /// <summary>
        /// Icon Color
        /// </summary>
        public required string? IconColor { get; set; }

        /// <summary>
        /// Localization Name
        /// </summary>
        public required string LocalizationName { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public required int Count { get; set; }

        /// <summary>
        /// Maximum Stack Allowed
        /// </summary>
        public required int MaxStackAllowed { get; set; }

        /// <summary>
        /// Quality
        /// </summary>
        public required int? Quality { get; set; }

        /// <summary>
        /// Quality color
        /// </summary>
        public required string? QualityColor { get; set; }

        /// <summary>
        /// Use times
        /// </summary>
        public required float UseTimes { get; set; }

        /// <summary>
        /// Maximum use times
        /// </summary>
        public required int MaxUseTimes { get; set; }

        /// <summary>
        /// Is this a module?
        /// </summary>
        public required bool IsMod { get; set; }

        /// <summary>
        /// Is this a block?
        /// </summary>
        public required bool IsBlock { get; set; }

        /// <summary>
        /// Parts
        /// </summary>
        public required InvItemDto?[]? Parts { get; set; }
    }
}