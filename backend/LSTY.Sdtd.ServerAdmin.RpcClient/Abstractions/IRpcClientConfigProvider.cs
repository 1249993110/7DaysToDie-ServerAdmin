using LSTY.Sdtd.ServerAdmin.RpcClient.Models;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions
{
    public interface IRpcClientConfigProvider
    {
        Task<IEnumerable<RpcClientConfig>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
