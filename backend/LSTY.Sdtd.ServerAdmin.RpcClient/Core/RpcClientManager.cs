using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Clients;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Core
{
    public class RpcClientManager : IAsyncDisposable
    {
        private readonly ConcurrentDictionary<Guid, IRpcClient> _clients = new();
        private readonly ICustomLoggerFactory _customLoggerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRpcClientConfigProvider _configProvider;

        public RpcClientManager(
            ICustomLoggerFactory customLoggerFactory,
            IServiceProvider serviceProvider,
            IRpcClientConfigProvider configProvider)
        {
            _customLoggerFactory = customLoggerFactory;
            _serviceProvider = serviceProvider;
            _configProvider = configProvider;
        }

        public ICollection<IRpcClient> GetAllClients() => _clients.Values;

        public async Task<int> LoadClientsFromConfigAsync(CancellationToken cancellationToken)
        {
            int count = 0;
            var configs = await _configProvider.GetAllAsync(cancellationToken);
            foreach (var config in configs)
            {
                var client = CreateJsonRpcClient(config);
                if(_clients.TryAdd(config.Id, client))
                {
                    count++;
                }
            }

            return count;
        }

        public async Task RemoveClientAsync(Guid id)
        {
            if (_clients.TryRemove(id, out var client))
            {
                await client.DisposeAsync();

                var logger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.RpcClientManager, id);
                await logger.LogInformationAsync($"Rpc Client {client.Config.Name} disposed.");
            }
        }

        private JsonRpcClient CreateJsonRpcClient(RpcClientConfig config)
        {
            return ActivatorUtilities.CreateInstance<JsonRpcClient>(_serviceProvider, config);
        }

        public async Task AddOrUpdateClientAsync(RpcClientConfig config)
        {
            var client = CreateJsonRpcClient(config);

            IRpcClient? oldClient = null;

            _clients.AddOrUpdate(config.Id, client, (key, _oldClient) =>
            {
                oldClient = _oldClient;
                return client;
            });

            if (oldClient != null)
            {
                await oldClient.DisposeAsync();
            }

            var logger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.RpcClientManager, config.Id);
            await logger.LogInformationAsync($"Rpc Client [{config.Name}] added or updated.");
        }

        public bool TryGetClient(Guid id, [MaybeNullWhen(false)] out IRpcClient rpcClient)
        {
            return _clients.TryGetValue(id, out rpcClient);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var client in _clients.Values)
            {
                await client.DisposeAsync();
            }

            _clients.Clear();
        }
    }
}
