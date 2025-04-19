using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Models
{
    public class RpcClientConfig
    {
        public required string Id { get; set; }
        public required string Url { get; set; }
        public required X509Certificate2 Certificate { get; set; }
    }
}
