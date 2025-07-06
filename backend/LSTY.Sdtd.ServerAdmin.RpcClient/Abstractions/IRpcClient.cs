using LSTY.Sdtd.ServerAdmin.RpcClient.Clients;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions
{
    public interface IRpcClient : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        string Url { get; }
        ConnectionState State { get; }

        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        bool TryGetProxy<TProxy>(out TProxy? proxy) where TProxy : class, IProxy;
    }
}
