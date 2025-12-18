namespace LSTY.Sdtd.ServerAdmin.Shared.Dtos
{
    public class CpuTimesDto
    {
        public required ulong IdleTime { get; set; }
        public required ulong KernelTime { get; set; }
        public required ulong UserTime { get; set; }
    }
}
