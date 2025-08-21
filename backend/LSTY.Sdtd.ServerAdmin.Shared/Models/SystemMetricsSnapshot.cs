namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class SystemMetricsSnapshot
    {
        /// <summary>
        /// The UTC timestamp when the data was collected.
        /// </summary>
        public required DateTime Timestamp { get; set; }

        /// <summary>
        /// CPU time information.
        /// </summary>
        public required CpuTimes? CpuTimes { get; set; }

        /// <summary>
        /// Memory usage information.
        /// </summary>
        public required MemoryInfo? MemoryInfo { get; set; }

        /// <summary>
        /// List of disk I/O and space information (one for each disk).
        /// </summary>
        public required List<DiskInfo> DiskInfos { get; set; }

        /// <summary>
        /// List of network interface traffic information (one for each interface).
        /// </summary>
        public required List<NetworkInfo> NetworkInfos { get; set; }
    }
}
