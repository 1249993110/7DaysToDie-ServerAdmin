using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public enum ChatRecordQueryOrder
    {
        /// <summary>
        /// 
        /// </summary>
        CreatedOn,
    }

    /// <summary>
    /// Chat Record Query
    /// </summary>
    public class ChatMessageQueryDto : PaginationQuery<ChatRecordQueryOrder>
    {
        /// <summary>
        /// Start Date Time
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// End Date Time
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Chat Type
        /// </summary>
        public ChatType? ChatType { get; set; }

        /// <summary>
        /// Gets or sets the ID or name of the sender to search for.
        /// </summary>
        public string? SenderIdOrName { get; set; }
    }
}
