using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageQueryDto : PaginationQuery<CreatedAtQueryOrder>
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
        /// The ID or name of the sender to search for.
        /// </summary>
        public string? SenderIdOrName { get; set; }
    }
}
