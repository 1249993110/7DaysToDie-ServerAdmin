﻿using LSTY.Sdtd.ServerAdmin.Shared.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace LSTY.Sdtd.ServerAdmin.Helpers
{
    internal static class DeviceHelper
    {
        #region CPU
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetSystemTimes(out FILETIME lpIdleTime, out FILETIME lpKernelTime, out FILETIME lpUserTime);

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;

            public ulong ToULong() => ((ulong)dwHighDateTime << 32) | dwLowDateTime;
        }

        public static CpuTimes? GetCpuTimes()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if(GetSystemTimes(out var idleTime1, out var kernelTime1, out var userTime1) == false)
                {
                    return null;
                }

                return new CpuTimes()
                {
                    IdleTime = idleTime1.ToULong(),
                    KernelTime = kernelTime1.ToULong(),
                    UserTime = userTime1.ToULong()
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // For Linux, you would typically read from /proc/stat
                var lines = File.ReadAllLines("/proc/stat");
                var cpuLine = lines.FirstOrDefault(line => line.StartsWith("cpu "));

                if (cpuLine == null)
                {
                    return null;
                }

                var parts = cpuLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Check if there are enough data fields (at least until iowait)
                if (parts.Length < 8)
                {
                    return null;
                }

                try
                {
                    ulong user = ulong.Parse(parts[1]);
                    ulong nice = ulong.Parse(parts[2]);
                    ulong system = ulong.Parse(parts[3]);
                    ulong idle = ulong.Parse(parts[4]);
                    ulong iowait = ulong.Parse(parts[5]);
                    ulong irq = ulong.Parse(parts[6]);
                    ulong softirq = ulong.Parse(parts[7]);

                    var cpuTimes = new CpuTimes()
                    {
                        UserTime = user + nice,
                        KernelTime = system + irq + softirq,
                        IdleTime = idle + iowait,
                    };

                    return cpuTimes;
                }
                catch
                {
                    return null;
                }
            }

            throw new PlatformNotSupportedException("Unsupported platform for CPU usage retrieval.");
        }
        #endregion

        #region Memory
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {

            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        public static MemoryInfo? GetMemoryInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var memStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(ref memStatus))
                {
                    return new MemoryInfo()
                    {
                        TotalPhysicalMemory = memStatus.ullTotalPhys,
                        AvailablePhysicalMemory = memStatus.ullAvailPhys,
                        TotalVirtualMemory = memStatus.ullTotalVirtual,
                        AvailableVirtualMemory = memStatus.ullAvailVirtual,
                        UsedPercentage = memStatus.dwMemoryLoad
                    };
                }
                else
                {
                    return null;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    var memInfo = ParseProcMemInfo();

                    if (memInfo.TryGetValue("MemTotal", out ulong total) == false ||
                        memInfo.TryGetValue("MemAvailable", out ulong available) == false)
                    {
                        return null;
                    }

                    ulong used = total - available;
                    uint usedPercentage = (uint)((double)used / total * 100);

                    // Swap info is optional but good to have
                    memInfo.TryGetValue("SwapTotal", out ulong totalSwap);
                    memInfo.TryGetValue("SwapFree", out ulong freeSwap);

                    return new MemoryInfo()
                    {
                        TotalPhysicalMemory = total * 1024,
                        AvailablePhysicalMemory = available * 1024,
                        TotalVirtualMemory = totalSwap * 1024,
                        AvailableVirtualMemory = freeSwap * 1024,
                        UsedPercentage = usedPercentage
                    };
                }
                catch
                {
                    return null;
                }
            }

            throw new PlatformNotSupportedException("Unsupported platform for memory status retrieval.");
        }

        private static Dictionary<string, ulong> ParseProcMemInfo()
        {
            var info = new Dictionary<string, ulong>();
            var lines = File.ReadAllLines("/proc/meminfo");

            foreach (var line in lines)
            {
                var parts = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    var key = parts[0];
                    var valueString = parts[1].Replace(" kB", string.Empty).Trim();
                    if (ulong.TryParse(valueString, out ulong value))
                    {
                        info[key] = value;
                    }
                }
            }
            return info;
        }
        #endregion

        #region Disk

        public static IEnumerable<DiskInfo> GetDiskInfos()
        {
            var disks = DriveInfo.GetDrives()
                .Where(x => x.DriveType == DriveType.Fixed && x.TotalSize != 0 && x.DriveFormat != "overlay");

            return disks.Select(x => new DiskInfo() 
            {
                DriveType = x.DriveType,
                DriveFormat = x.DriveFormat,
                FreeSpace = x.AvailableFreeSpace,
                Name = x.Name,
                RootPath = x.RootDirectory.FullName,
                TotalSize = x.TotalSize
            }).Distinct(new DiskInfoEquality());
        }

        private class DiskInfoEquality : IEqualityComparer<DiskInfo>
        {
            public bool Equals(DiskInfo? x, DiskInfo? y)
            {
                return x?.Name == y?.Name;
            }

            public int GetHashCode(DiskInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }
        #endregion

        #region Network
        public static List<NetworkInfo> GetNetworkInfos()
        {
            var hostName = Dns.GetHostName();
            var hostAddrs = Dns.GetHostAddresses(hostName).Where(i => i.AddressFamily == AddressFamily.InterNetwork).ToHashSet();
            
            var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            var result = new List<NetworkInfo>(allNetworkInterfaces.Length);
            foreach (var item in allNetworkInterfaces)
            {
                if (item.OperationalStatus != OperationalStatus.Up || item.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                var ipAddresses = item.GetIPProperties().UnicastAddresses.Select(i => i.Address);
                if (hostAddrs.Overlaps(ipAddresses))
                {
                    var ipStatistics = item.GetIPv4Statistics();
                    var networkInfo = new NetworkInfo()
                    {
                        Id = item.Id,
                        Mac = BitConverter.ToString(item.GetPhysicalAddress().GetAddressBytes()),
                        Name = item.Name,
                        Trademark = item.Description,
                        BytesReceived = ipStatistics.BytesReceived,
                        BytesSent = ipStatistics.BytesSent,
                        NetworkType = item.NetworkInterfaceType,
                        Speed = item.Speed,
                        IpAddresses = ipAddresses.Select(i => i.ToString()).ToArray(),
                    };
                    result.Add(networkInfo);
                }
            }

            return result;
        }
        #endregion

        #region SystemPlatform
        public static SystemPlatformInfo GetSystemPlatformInfo()
        {
            return new SystemPlatformInfo()
            {
                DeviceModel = UnityEngine.Device.SystemInfo.deviceModel,
                DeviceName = UnityEngine.Device.SystemInfo.deviceName,
                DeviceType = UnityEngine.Device.SystemInfo.deviceType.ToString(),
                DeviceUniqueIdentifier = UnityEngine.Device.SystemInfo.deviceUniqueIdentifier,
                OperatingSystem = UnityEngine.Device.SystemInfo.operatingSystem,
                OperatingSystemFamily = UnityEngine.Device.SystemInfo.operatingSystemFamily.ToString(),
                ProcessorCount = Environment.ProcessorCount, // UnityEngine.Device.SystemInfo.processorCount,
                ProcessorFrequency = UnityEngine.Device.SystemInfo.processorFrequency,
                ProcessorType = UnityEngine.Device.SystemInfo.processorType,
                SystemMemorySize = UnityEngine.Device.SystemInfo.systemMemorySize,
                FrameworkVersion = Environment.Version.ToString(),
                UserName = Environment.UserName,
            };
        }
        #endregion
    }
}
