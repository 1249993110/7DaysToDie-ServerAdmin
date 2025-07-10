using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Providers
{
    /// <summary>
    /// Provides the configuration for RPC clients.
    /// </summary>
    public class RpcClientConfigProvider : IRpcClientConfigProvider
    {
        private readonly ILogger<RpcClientConfigProvider> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="RpcClientConfigProvider"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public RpcClientConfigProvider(ILogger<RpcClientConfigProvider> logger)
        {
            _logger = logger;
        }

        async Task<IEnumerable<RpcClientConfig>> IRpcClientConfigProvider.GetAllAsync(CancellationToken cancellationToken)
        {
            var configs = new List<RpcClientConfig>();

            long count = await Db.QueryCount<GameServerConfig>().GetAsync();
            if (count == 0)
            {
                byte[] pfxFile = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "cert.pfx"));

                var entity = new GameServerConfig()
                {
                    Id = Guid.NewGuid(),
                    Host = "7dtdserver.local",
                    Port = 8088,
                    IsEnabled = true,
                    Name = "Test",
                    PfxFile = pfxFile,
                    PfxPassword = null,
                    UserId = "admin",
                };

                await Db.Insert(entity).ExecuteAsync();
            }

            var gameServerConfigs = await Db.Query<GameServerConfig>().WhereEq(p => p.IsEnabled, true).GetListAsync();
            foreach (var config in gameServerConfigs)
            {
                var rpcClientConfig = new RpcClientConfig()
                {
                    Id = config.Id,
                    Host = config.Host,
                    Port = config.Port,
                    PfxFile = config.PfxFile,
                    PfxPassword = config.PfxPassword,
                    Name = config.Name,
                };

                configs.Add(rpcClientConfig);
            }

            return configs;
        }
    }
}
