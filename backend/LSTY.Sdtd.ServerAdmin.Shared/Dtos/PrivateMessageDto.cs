namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    public class PrivateMessageDto : GlobalMessageDto
    {
        /// <summary>
        /// The target player id or name.
        /// </summary>
        public required string TargetPlayerIdOrName { get; set; }
    }
}
