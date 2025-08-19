namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class MemoryInfo
    {
        public required ulong TotalPhysicalMemory { get; set; }

        public required ulong AvailablePhysicalMemory { get; set; }

        public required uint UsedPercentage { get; set; }

        public required ulong TotalVirtualMemory { get; set; }

        public required ulong AvailableVirtualMemory { get; set; }
    }
}
