using Serilog.Sinks.File.Archive;
using System.IO.Compression;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    /// <summary>
    /// Provides custom hook configurations for the Serilog logging system
    /// </summary>
    public class SerilogHooks
    {
        /// <summary>
        /// Gets an archive hook configuration instance that sets log files to be retained for 365 days
        /// with the smallest compression level
        /// </summary>
        public static ArchiveHooks MyArchiveHooks => new ArchiveHooks(365, CompressionLevel.SmallestSize);
    }
}
