namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    /// <summary>
    /// Owned Entity
    /// </summary>
    public class OwnedEntity
    {
        public required int Id { get; set; }
        public required int ClassId { get; set; }
        public required PositionDto LastKnownPosition { get; set; }
        public required bool HasLastKnownPosition { get; set; }
    }
}
