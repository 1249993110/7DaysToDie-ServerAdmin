namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class SystemPlatformInfo
    {
        public required string DeviceModel { get; set; }
        public required string DeviceName { get; set; }
        public required string DeviceType { get; set; }
        public required string DeviceUniqueIdentifier { get; set; }
        public required string OperatingSystem { get; set; }
        public required string OperatingSystemFamily { get; set; }
        public required int ProcessorCount { get; set; }
        public required int ProcessorFrequency { get; set; }
        public required string ProcessorType { get; set; }
        public required int SystemMemorySize { get; set; }

        /// <summary>
        /// .NET Fx/Core version
        /// <para>
        /// ex:<br />
        /// 3.1.9
        /// </para>
        /// </summary>
        public required string FrameworkVersion { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public required string UserName { get; set; }
    }
}
