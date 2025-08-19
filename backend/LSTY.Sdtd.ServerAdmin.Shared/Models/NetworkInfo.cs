using System.Net.NetworkInformation;

namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    public class NetworkInfo
    {
        public required string Id { get; set; }
        public required string Mac { get; set; }
        public required string Name { get; set; }
        public required string Trademark { get; set; }
        public required NetworkInterfaceType NetworkType { get; set; }
        public required long Speed { get; set; }
        public required string[] IpAddresses { get; set; }

        public required long BytesReceived { get; set; }
        public required long BytesSent { get; set; }
    }
}
