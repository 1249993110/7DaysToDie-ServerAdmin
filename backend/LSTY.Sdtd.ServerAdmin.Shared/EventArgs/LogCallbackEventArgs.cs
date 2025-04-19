using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class LogCallbackEventArgs : System.EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? StackTrace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required LogLevel LogLevel { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }
    }
}