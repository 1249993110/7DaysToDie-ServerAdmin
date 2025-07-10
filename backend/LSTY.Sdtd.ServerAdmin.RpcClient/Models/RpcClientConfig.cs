namespace LSTY.Sdtd.ServerAdmin.RpcClient.Models
{
    public class RpcClientConfig
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required byte[] PfxFile { get; set; }
        public string? PfxPassword { get; set; }
    }
}
