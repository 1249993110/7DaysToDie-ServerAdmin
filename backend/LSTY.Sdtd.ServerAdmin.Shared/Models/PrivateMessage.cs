namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class PrivateMessage : GlobalMessage
    {
        /// <summary>
        /// The target player id or name.
        /// </summary>
        public required string TargetPlayerIdOrName { get; set; }
    }
}
