namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    public class GlobalMessageDto
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
