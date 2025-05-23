﻿using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using MongoDB.Entities;
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

            var count = await DB.CountAsync<GameServerConfig>(cancellation: cancellationToken);
            if (count == 0)
            {
                var config = new GameServerConfig()
                {
                    Ip = "172.16.168.25",
                    Port = 8088,
                    IsEnabled = true,
                    Name = "Test",
                    PfxPassword = null,
                    UserId = "admin",
                };

                await config.SaveAsync(cancellation: cancellationToken);

                using var fileStream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, GameServerConfig.PfxFileName));
                await config.Data.UploadAsync(fileStream, cancellation: cancellationToken);
            }

            var gameServerConfigs = await DB.Find<GameServerConfig>().ManyAsync(p => p.IsEnabled, cancellationToken);
            foreach (var config in gameServerConfigs)
            {
                try
                {
                    using var ms = new MemoryStream((int)config.FileSize);
                    await config.Data.DownloadAsync(ms, cancellation: cancellationToken);
                    byte[] data = ms.ToArray();

                    var rpcClientConfig = new RpcClientConfig()
                    {
                        Id = config.ID,
                        Url = $"tcp://{config.Ip}:{config.Port}",
                        Certificate = X509CertificateLoader.LoadPkcs12(data, config.PfxPassword),
                        Name = config.Name,
                    };

                    configs.Add(rpcClientConfig);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load rpc client config with ID: {Id}", config.ID);
                }
            }

            return configs;

        }
    }
}
