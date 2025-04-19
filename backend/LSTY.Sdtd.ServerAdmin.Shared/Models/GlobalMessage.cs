namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class GlobalMessage
    {
        /// <summary>
        /// Message
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Sender Name
        /// </summary>
        public string? SenderName { get; set; }
    }
}
