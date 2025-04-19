using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Clients;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Core
{
    public class RpcClientManager
    {
        private readonly ConcurrentDictionary<string, IRpcClient> _clients = new();
        private readonly ILogger<RpcClientManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRpcClientConfigProvider _configProvider;

        public RpcClientManager(
            ILogger<RpcClientManager> logger,
            IServiceProvider serviceProvider,
            IRpcClientConfigProvider configProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configProvider = configProvider;
        }

        public ICollection<IRpcClient> GetAllClients() => _clients.Values;

        public async Task LoadClientsFromConfigAsync(CancellationToken cancellationToken)
        {
            var configs = await _configProvider.GetAllAsync(cancellationToken);
            foreach (var config in configs)
            {
                var client = CreateJsonRpcClient(config);
                if (_clients.TryAdd(config.Id, client) == false)
                {
                    _logger.LogWarning("Client with name {Name} already exists. Skipping...", config.Id);
                }
            }

            _logger.LogInformation("Loaded {ClientCount} RPC clients from config.", _clients.Count);
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
            if (_clients.TryRemove(id, out var client))
            {
                await client.DisconnectAsync();
                client.Dispose();
                _logger.LogInformation("Client {Id} disposed.", id);
            }
            else
            {
                _logger.LogWarning("Client {Id} not found.", id);
            }
        }

        private JsonRpcClient CreateJsonRpcClient(RpcClientConfig config)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<JsonRpcClient>>();
            var appLifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            return new JsonRpcClient(config, logger, appLifetime, mediator);
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
            _logger.LogInformation("Client {Id} added or updated.", config.Id);
        }

        public bool TryGetClient(string id, out IRpcClient? rpcClient)
        {
            return _clients.TryGetValue(id, out rpcClient);
        }
    }
}
