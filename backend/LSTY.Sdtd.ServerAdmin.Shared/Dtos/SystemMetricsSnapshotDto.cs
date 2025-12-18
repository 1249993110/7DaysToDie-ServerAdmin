namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    public class SystemMetricsSnapshotDto
    {
        /// <summary>
        /// The UTC timestamp when the data was collected.
        /// </summary>
        public required DateTime Timestamp { get; set; }

        /// <summary>
        /// CPU time information.
        /// </summary>
        public required CpuTimesDto? CpuTimes { get; set; }

        /// <summary>
        /// Memory usage information.
        /// </summary>
        public required MemoryInfoDto? MemoryInfo { get; set; }

        /// <summary>
        /// List of disk I/O and space information (one for each disk).
        /// </summary>
        public required List<DiskInfoDto> DiskInfos { get; set; }

        /// <summary>
        /// List of network interface traffic information (one for each interface).
        /// </summary>
        public required List<NetworkInfoDto> NetworkInfos { get; set; }
    }
}
