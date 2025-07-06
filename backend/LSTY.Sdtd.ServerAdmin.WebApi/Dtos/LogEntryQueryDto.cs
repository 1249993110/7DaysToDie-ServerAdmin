using LSTY.Sdtd.ServerAdmin.Data.Enums;
using LogLevel = LSTY.Sdtd.ServerAdmin.Data.Enums.LogLevel;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class LogEntryQueryDto : PaginationQuery<CreatedAtQueryOrder>
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
        /// Service Module
        /// </summary>
        public ServiceModule? ServiceModule { get; set; }

        /// <summary>
        /// Log Level
        /// </summary>
        public LogLevel? LogLevel { get; set; }
    }
}
