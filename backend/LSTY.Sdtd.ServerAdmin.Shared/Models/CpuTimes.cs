namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class CpuTimes
    {
        public required ulong IdleTime { get; set; }
        public required ulong KernelTime { get; set; }
        public required ulong UserTime { get; set; }
    }
}
