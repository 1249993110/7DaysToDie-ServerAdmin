using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions
{
    public interface IRpcClient : IAsyncDisposable
    {
        RpcClientConfig Config { get; }

        bool IsConnected { get; }

        bool TryGetProxy<TProxy>([MaybeNullWhen(false)] out TProxy proxy) where TProxy : class, IProxy;
    }
}
