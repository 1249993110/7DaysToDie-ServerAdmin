using LSTY.Sdtd.ServerAdmin.Helpers;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using System.Web.Http;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Provides an interface for querying device and system information.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Devices")]
    public class DevicesController : ApiController
    {
        /// <summary>
        /// Get cpu times.
        /// </summary>
        [HttpGet]
        [Route("CpuTimes")]
        public CpuTimes? GetCpuTimes()
        {
            return DeviceHelper.GetCpuTimes();
        }

        /// <summary>
        /// Get memory info.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("MemoryInfo")]
        public MemoryInfo? GetMemoryInfo()
        {
            return DeviceHelper.GetMemoryInfo();
        }

        /// <summary>
        /// Get disk infos.
        /// </summary>
        [HttpGet]
        [Route("DiskInfos")]
        public IEnumerable<DiskInfo> GetDiskInfos()
        {
            return DeviceHelper.GetDiskInfos();
        }

        /// <summary>
        /// Get network infos.
        /// </summary>
        [HttpGet]
        [Route("NetworkInfos")]
        public List<NetworkInfo> GetNetworkInfos()
        {
            return DeviceHelper.GetNetworkInfos();
        }

        /// <summary>
        /// Get system platform info.
        /// </summary>
        [HttpGet]
        [Route("SystemPlatformInfo")]
        public SystemPlatformInfo GetSystemPlatformInfo()
        {
            return DeviceHelper.GetSystemPlatformInfo();
        }

        /// <summary>
        /// Get system metrics snapshot.
        /// </summary>
        [HttpGet]
        [Route("SystemMetricsSnapshot")]
        public SystemMetricsSnapshot GetSystemMetricsSnapshot()
        {
            return new SystemMetricsSnapshot()
            {
                Timestamp = DateTime.UtcNow,
                CpuTimes = DeviceHelper.GetCpuTimes(),
                MemoryInfo = DeviceHelper.GetMemoryInfo(),
                DiskInfos = DeviceHelper.GetDiskInfos(),
                NetworkInfos = DeviceHelper.GetNetworkInfos()
            };
        }
    }
}
