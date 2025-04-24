using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Clients;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Core
{
    public class RpcClientManager
    {
        private readonly ConcurrentDictionary<string, IRpcClient> _clients = new();
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

        public async Task StartAllAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var client in _clients.Values)
            {
                tasks.Add(client.ConnectAsync(cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        public async Task StopAllAsync()
        {
            var tasks = new List<Task>();
            foreach (var client in _clients.Values)
            {
                tasks.Add(client.DisconnectAsync());
            }

            await Task.WhenAll(tasks);
        }

        public async Task RemoveClientAsync(string id)
        {
            var logger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.RpcClientManager, id);

            if (_clients.TryRemove(id, out var client))
            {
                await client.DisconnectAsync();
                client.Dispose();
                await logger.LogInformationAsync($"Rpc Client {client.Name} disposed.");
            }
            else
            {
                await logger.LogWarningAsync($"Rpc Client {id} not found.");
            }
        }

        private JsonRpcClient CreateJsonRpcClient(RpcClientConfig config)
        {
            return ActivatorUtilities.CreateInstance<JsonRpcClient>(_serviceProvider, config);
        }

        public async Task AddOrUpdateClientAsync(RpcClientConfig config)
        {
            var client = CreateJsonRpcClient(config);
            _clients.AddOrUpdate(config.Id, (key, client) =>
            {
                return client;
            }, (key, oldValue, client) =>
            {
                oldValue.Dispose();
                return client;
            }, client);

            await client.ConnectAsync();

            var logger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.RpcClientManager, config.Id);
            await logger.LogInformationAsync($"Rpc Client {config.Name} added or updated.");
        }

        public bool TryGetClient(string id, out IRpcClient? rpcClient)
        {
            return _clients.TryGetValue(id, out rpcClient);
        }
    }
}
