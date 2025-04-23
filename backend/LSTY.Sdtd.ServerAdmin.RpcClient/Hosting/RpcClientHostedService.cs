using LSTY.Sdtd.ServerAdmin.RpcClient.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Hosting
{
    public class RpcClientHostedService : IHostedService
    {
        private readonly RpcClientManager _manager;
        private readonly ILogger<RpcClientHostedService> _logger;

        public RpcClientHostedService(RpcClientManager manager, ILogger<RpcClientHostedService> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Name} starting...", nameof(RpcClientHostedService));

            int count = await _manager.LoadClientsFromConfigAsync(cancellationToken);
            _logger.LogInformation("Loaded {ClientCount} RPC clients from config.", count);

            await _manager.StartAllAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _manager.StopAllAsync();
            _logger.LogInformation("{Name} stopped.", nameof(RpcClientHostedService));
        }
    }
}
