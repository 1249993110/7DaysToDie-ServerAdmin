namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    public class PlayerProfileDto
    {
        public required bool IsMale { get; set; }
        public required string RaceName { get; set; }
        public required int VariantNumber { get; set; }
        public required string EyeColor { get; set; }
        public required string HairName { get; set; }
        public required string HairColor { get; set; }
        public required string MustacheName { get; set; }
        public required string ChopsName { get; set; }
        public required string BeardName { get; set; }
        public required string ProfileArchetype { get; set; }
        public required string EntityClassName { get; set; }
    }
}
